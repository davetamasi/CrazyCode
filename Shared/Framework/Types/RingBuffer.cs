﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// Represents a fixted length ring buffer to store a specified maximal count of items within.
/// </summary>
/// <typeparam name="T">The generic type of the items stored within the ring buffer.</typeparam>
[DebuggerDisplay( "Count = {Count}" )]
public class RingBuffer<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
	#region Fields and Constructors

	/// <summary>
	/// the internal buffer
	/// </summary>
	private readonly T[] _buffer;

	/// <summary>
	/// The all-over position within the ring buffer. The position increases continously by adding
	/// new items to the buffer. This value is needed to calculate the current relative position
	/// within the buffer.
	/// </summary>
	private Int32 _position;

	/// <summary>
	/// The current version of the buffer, this is required for a correct exception handling while
	/// enumerating over the items of the buffer.
	/// </summary>
	private Int64 _version;

	/// <summary>
	/// Creates a new instance of a <see cref="RingBuffer&lt;T&gt;"/> with a specified cache size.
	/// </summary>
	/// <param name="capacity">The maximal count of items to be stored within the ring buffer.</param>
	public RingBuffer( Int32 capacity )
	{
		// validate capacity
		if( capacity <= 0 )
		{
			throw new ArgumentException( "Must be greater than zero", "capacity" );
		}
		// set capacity and init the cache
		this.Capacity = capacity;
		_buffer = new T[ capacity ];
	}

	#endregion

	#region Properties

	/// <summary>
	/// Gets the maximal count of items within the ring buffer
	/// </summary>
	public Int32 Capacity { get; private set; }

	/// <summary>
	/// Get the current count of items within the ring buffer.
	/// </summary>
	public Int32 Count { get; private set; }

	/// <summary>
	/// Gets or sets an item for a specified position within the ring buffer.
	/// </summary>
	/// <param name="index">The position to get or set an item.</param>
	/// <returns>The fond item at the specified position within the ring buffer.</returns>
	/// <exception cref="IndexOutOfRangeException"></exception>
	public T this[ Int32 index ]
	{
		get
		{
			if( index < 0 || index >= Count )
			{
				throw new IndexOutOfRangeException();
			}

			// calculate the relative position within the rolling base array
			Int32 index2 = ( _position - Count + index ) % this.Capacity;
			return _buffer[ index2 ];
		}
		set
		{
			this.Insert( index, value );
		}
	}

	#endregion

	/// <summary>
	/// Adds a new item to the buffer.
	/// </summary>
	/// <param name="item">The item to be added to the buffer.</param>
	public void Add( T item )
	{
		// avoid an arithmetic overflow
		if( _position == Int32.MaxValue )
		{
			_position = _position % this.Capacity;
		}

		// add a new item to the current relative position within the buffer and increase the position
		_buffer[ _position++ % this.Capacity ] = item;
		// increase the count if capacity is not yet reached
		if( Count < this.Capacity ) Count++;
		// buffer changed; next version
		_version++;
	}

	/// <summary>
	/// Clears the whole buffer and releases all referenced objects currently stored within the buffer.
	/// </summary>
	public void Clear()
	{
		for( Int32 i = 0; i < Count; i++ )
			_buffer[ i ] = default( T );
		_position = 0;
		Count = 0;
		_version++;
	}

	/// <summary>
	/// Determines if a specified item is currently present within the buffer.
	/// </summary>
	/// <param name="item">The item to search for within the current buffer.</param>
	/// <returns>
	/// True if the specified item is currently present within the buffer; otherwise false.
	/// </returns>
	public Boolean Contains( T item )
	{
		Int32 index = IndexOf( item );
		return index != -1;
	}

	/// <summary>
	/// Copies the current items within the buffer to a specified array.
	/// </summary>
	/// <param name="array">The target array to copy the items of the buffer to.</param>
	/// <param name="arrayIndex">The start position witihn the target array to start copying.</param>
	public void CopyTo( T[] array, Int32 arrayIndex )
	{
		for( Int32 i = 0; i < Count; i++ )
		{
			array[ i + arrayIndex ] = _buffer[ ( _position - Count + i ) % this.Capacity ];
		}
	}

	/// <summary>
	/// Gets an enumerator over the current items within the buffer.
	/// </summary>
	/// <returns>An enumerator over the current items within the buffer.</returns>
	public IEnumerator<T> GetEnumerator()
	{
		Int64 version = _version;
		for( Int32 i = 0; i < Count; i++ )
		{
			if( version != _version )
				throw new InvalidOperationException( "Collection changed" );
			yield return this[ i ];
		}
	}

	/// <summary>
	/// Gets the position of a specied item within the ring buffer.
	/// </summary>
	/// <param name="item">The item to get the current position for.</param>
	/// <returns>
	/// The zero based index of the found item within the buffer. If the item was not present within
	/// the buffer, this method returns -1.
	/// </returns>
	public Int32 IndexOf( T item )
	{
		for( Int32 i = 0; i < Count; i++ )
		{
			// get the item at the relative position within the internal array
			T item2 = _buffer[ ( _position - Count + i ) % this.Capacity ];

			// if both items are null, return true
			if( null == item && null == item2 )
			{
				return i;
			}

			// if equal return the position
			if( item != null && item.Equals( item2 ) )
			{
				return i;
			}
		}
		// nothing found
		return -1;
	}

	/// <summary>
	/// Inserts an item at a specified position into the buffer.
	/// </summary>
	/// <param name="index">The position within the buffer to add the new item.</param>
	/// <param name="item">The new item to be added to the buffer.</param>
	/// <exception cref="IndexOutOfRangeException"></exception>
	/// <remarks>
	/// If the specified index is equal to the current count of items within the buffer, the
	/// specified item will be added.
	/// 
	/// <b>Warning</b> Frequent usage of this method might become a bad idea if you are working with
	/// a large buffer capacity. The insertion of an item at a specified position within the buffer
	/// causes causes all present items below the specified position to be moved one position.
	/// </remarks>
	public void Insert( Int32 index, T item )
	{
		// validate index
		if( index < 0 || index > Count )
			throw new IndexOutOfRangeException();
		// add if index equals to count
		if( index == Count )
		{
			Add( item );
			return;
		}

		// get the maximal count of items to be moved
		Int32 count = Math.Min( this.Count, this.Capacity - 1 ) - index;
		// get the relative position of the new item within the buffer
		Int32 index2 = ( _position - Count + index ) % this.Capacity;

		// move all items below the specified position
		for( Int32 i = index2 + count; i > index2; i-- )
		{
			Int32 to = i % this.Capacity;
			Int32 from = ( i - 1 ) % this.Capacity;
			_buffer[ to ] = _buffer[ from ];
		}

		// set the new item
		_buffer[ index2 ] = item;

		// adjust storage information
		if( this.Count < this.Capacity )
		{
			Count++;
			_position++;
		}
		// buffer changed; next version
		_version++;
	}

	/// <summary>
	/// Removes a specified item from the current buffer.
	/// </summary>
	/// <param name="item">The item to be removed.</param>
	/// <returns>
	/// True if the specified item was successfully removed from the buffer; otherwise false.
	/// </returns>
	/// <remarks>
	/// <b>Warning</b> Frequent usage of this method might become a bad idea if you are working with
	/// a large buffer capacity. The removing of an item requires a scan of the buffer to get the
	/// position of the specified item. If the item was found, the deletion requires a move of all
	/// items stored abouve the found position.
	/// </remarks>
	public Boolean Remove( T item )
	{
		// find the position of the specified item
		Int32 index = IndexOf( item );
		// item was not found; return false
		if( index == -1 )
		{
			return false;
		}
		
		// remove the item at the specified position
		this.RemoveAt( index );
		return true;
	}

	/// <summary>
	/// Removes an item at a specified position within the buffer.
	/// </summary>
	/// <param name="index">The position of the item to be removed.</param>
	/// <exception cref="IndexOutOfRangeException"></exception>
	/// <remarks>
	/// <b>Warning</b> Frequent usage of this method might become a bad idea if you are working with
	/// a large buffer capacity. The deletion requires a move of all items stored abouve the found position.
	/// </remarks>
	public void RemoveAt( Int32 index )
	{
		// validate the index
		if( index < 0 || index >= Count )
		{
			throw new IndexOutOfRangeException();
		}

		// move all items above the specified position one step closer to zeri
		for( Int32 i = index; i < Count - 1; i++ )
		{
			// get the next relative target position of the item
			Int32 to = ( _position - Count + i ) % this.Capacity;
			// get the next relative source position of the item
			Int32 from = ( _position - Count + i + 1 ) % this.Capacity;
			// move the item
			_buffer[ to ] = _buffer[ from ];
		}
		// get the relative position of the last item, which becomes empty after deletion and set
		// the item as empty
		Int32 last = ( _position - 1 ) % this.Capacity;
		_buffer[ last ] = default( T );

		// adjust storage information, and buffer changed; next version
		_position--;
		this.Count--;
		_version++;
	}

	/// <summary>
	/// Gets if the buffer is read-only. This method always returns false.
	/// </summary>
	Boolean ICollection<T>.IsReadOnly { get { return false; } }

	/// <summary>
	/// See generic implementation of <see cref="GetEnumerator"/>.
	/// </summary>
	/// <returns>See generic implementation of <see cref="GetEnumerator"/>.</returns>
	IEnumerator IEnumerable.GetEnumerator()
	{
		return this.GetEnumerator();
	}
}