using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Tamasi.Shared.Framework.Extensions
{
	public static class LinqExtensions
	{
		/// <summary>
		/// Enables dynamic sorting of a Linq query by passing in the sort column name
		/// as a string, like we're used to doing in ADO.Net. This is an extension of
		/// the Linq orderBy method.
		/// </summary>
		/// <param name="sortExpression">Name of column on which to sort. Sorry, multiple columns are not supported at this point.</param>
		/// <param name="sortDescending">True if the sort is to be in Descending order, false if in Ascending order.</param>
		/// <returns></returns>

		public static IQueryable<TEntity> OrderBy<TEntity>
		(
			this IQueryable<TEntity> source,
			string sortExpression,
			Boolean sortDescending ) where TEntity : class
		{
			Debug.Assert( source != null );
			Debug.Assert( !string.IsNullOrEmpty( sortExpression ) );

			var type = typeof( TEntity );
			string methodName = "OrderBy";

			// Check for descending sort order and append to the method name if necessary.
			if( sortDescending )
			{
				methodName += "Descending";
			}
			//
			// Get the type property of the sortExpression column,
			// and construct the expression parameters we will need
			var property = type.GetProperty( sortExpression );
			var parameter = Expression.Parameter( type, "p" );
			var propertyAccess = Expression.MakeMemberAccess( parameter, property );
			var orderByExp = Expression.Lambda( propertyAccess, parameter );

			// Create a MethodCallExpression using the methodName and the expression
			// parameters constructed above. This will then be used to create the
			// IQueryable entity, which we return to the caller.
			MethodCallExpression resultExp = Expression.Call
			(
				typeof( Queryable ),
				methodName,
				new Type[] { type, property.PropertyType },
				source.Expression,
				Expression.Quote( orderByExp )
			);

			return source.Provider.CreateQuery<TEntity>( resultExp );
		}

		public static ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>
		(
			this IEnumerable<TValue> source,
			Func<TValue, TKey> valueSelector )
		{
			return new ConcurrentDictionary<TKey, TValue>
			(
				source.ToDictionary( valueSelector )
			);
		}
	}
}