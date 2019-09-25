using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Tamasi.Shared.Framework.Extensions
{
	public static class QueryableChunked
	{
		public static IEnumerable<T> InRange<T, TValue>
		(
			this IQueryable<T> source,
			Expression<Func<T, TValue>> selector,
			Int32 blockSize,
			IEnumerable<TValue> values )
		{
			MethodInfo method = null;

			foreach( MethodInfo tmp in typeof( Enumerable ).GetMethods( BindingFlags.Public | BindingFlags.Static ) )
			{
				if( tmp.Name == "Contains" && tmp.IsGenericMethodDefinition
						&& tmp.GetParameters().Length == 2 )
				{
					method = tmp.MakeGenericMethod( typeof( TValue ) );
					break;
				}
			}

			if( method == null )
			{
				throw new InvalidOperationException( "Unable to locate Contains" );
			}

			foreach( TValue[] block in values.GetBlocks( blockSize ) )
			{
				var row = Expression.Parameter( typeof( T ), "row" );
				var member = Expression.Invoke( selector, row );
				var keys = Expression.Constant( block, typeof( TValue[] ) );
				var predicate = Expression.Call( method, keys, member );
				var lambda = Expression.Lambda<Func<T, Boolean>>(
					  predicate, row );
				foreach( T record in source.Where( lambda ) )
				{
					yield return record;
				}
			}
		}

		public static IEnumerable<T[]> GetBlocks<T>( this IEnumerable<T> source, Int32 blockSize )
		{
			List<T> list = new List<T>( blockSize );
			foreach( T item in source )
			{
				list.Add( item );
				if( list.Count == blockSize )
				{
					yield return list.ToArray();
					list.Clear();
				}
			}
			if( list.Count > 0 )
			{
				yield return list.ToArray();
			}
		}
	}

}
