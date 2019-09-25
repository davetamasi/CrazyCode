using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Tamasi.Shared.Framework.Extensions
{
	public static class CommonExtensions
	{
		public static string ToStringTrimIfLengthExceeds( this object foo, Int32 maxLength )
		{
			Debug.Assert( maxLength > 0 );

			string ret = null;

			if( foo != null )
			{
				string foos = foo.ToString();
				ret = ( foos.Length <= maxLength ) ? foos : foos.Substring( 0, maxLength - 1 );
			}

			return ret;
		}

		public static void Shuffle<T>( this IList<T> list )
		{
			Random rng = new Random();
			Int32 n = list.Count;
			while( n > 1 )
			{
				n--;
				Int32 k = rng.Next( n + 1 );
				T value = list[ k ];
				list[ k ] = list[ n ];
				list[ n ] = value;
			}
		}

		public static T To<T>( this object value )
		{
			Type t = typeof( T );

			if( t.IsGenericType && t.GetGenericTypeDefinition() == typeof( Nullable<> ) )
			{
				// Nullable type.

				if( value == null )
				{
					// you may want to do something different here.
					return default( T );
				}
				else
				{
					// Get the type that was made nullable.
					Type valueType = t.GetGenericArguments()[ 0 ];

					// Convert to the value type.
					object result = Convert.ChangeType( value, valueType );

					// Cast the value type to the nullable type.
					return ( T )result;
				}
			}
			else
			{
				// Not nullable.
				return ( T )Convert.ChangeType( value, typeof( T ) );
			}
		}

		/// <summary>
		/// Extension method to tests whether a string starts with any of the given strings
		/// </summary>
		/// <returns>
		/// Returns TRUE if the testString starts with any of the listed strings, FALSE otherwise
		/// </returns>
		public static string ReplaceAny( this string x, IEnumerable<string> oldValues, string newValue, Boolean ignoreCase = false )
		{
			if( oldValues == null ) throw new ArgumentNullException( nameof( oldValues ) );
			if( newValue == null ) throw new ArgumentNullException( nameof( newValue ) );

			foreach( string oldValue in oldValues )
			{
				x = x.Replace( oldValue, newValue );
				if( ignoreCase )
				{
					x = x.Replace( oldValue.ToUpper(), newValue );
					x = x.Replace( oldValue.ToLower(), newValue );
				}
			}

			return x;
		}

		public static Boolean AlmostEquals( this double double1, double double2, double precision )
		{
			return ( Math.Abs( double1 - double2 ) <= precision );
		}

		/// <summary>
		/// Returns the index of the first element in the sequence that satisfies a condition.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
		/// <param name="source">
		/// An <see cref="IEnumerable{T}"/> that contains the elements to apply the predicate to.
		/// </param>
		/// <param name="predicate">A function to test each element for a condition.</param>
		/// <returns>
		/// The zero-based index position of the first element of <paramref name="source"/> for
		/// which <paramref name="predicate"/> returns <see langword="true"/>; or -1 if <paramref
		/// name="source"/> is empty or no element satisfies the condition.
		/// </returns>
		public static Int32 IndexOf<TSource>
		(
			this IEnumerable<TSource> source,
			Func<TSource, Boolean> predicate )
		{
			Int32 i = 0;

			foreach( TSource element in source )
			{
				if( predicate( element ) )
					return i;

				i++;
			}

			return -1;
		}
	}
}