using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Transactions;
using System.Xml.Linq;

namespace Tamasi.Shared.Framework
{
	public static class LinqUtilities
	{
		#region LINQ to XML Helper Methods

		/// <summary>
		///
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static string Coalesce( XElement element )
		{
			string ret = null;

			if( element != null )
			{
				ret = element.Value;
			}

			return ret;
		}

		/// <summary>
		///
		/// </summary>
		public static string CoalesceStringAttribute( XElement element, XName attribute )
		{
			string ret = null;

			if( element != null && element.Attribute( attribute ) != null )
			{
				ret = element.Attribute( attribute ).Value;
			}

			return ret;
		}

		/// <summary>
		///
		/// </summary>
		public static Boolean CoalesceBooleanAttribute( XElement element, XName attribute, Boolean defaultVal = false )
		{
			Boolean ret = defaultVal;

			if( element == null
				|| element.Attribute( attribute ) == null
				|| !Boolean.TryParse( element.Attribute( attribute ).Value, out ret ) )
			{
				ret = defaultVal;
			}

			return ret;
		}

		#endregion

		/// <summary>
		/// Given a Linq DataContext and the constructed Query object,
		/// this method executes the Query using the Linq to DataSets
		/// extensions and returns a DataTable.
		/// </summary>
		/// <param name="ctx">an active Linq DataContext</param>
		/// <param name="query">the fully constructed Linq query object</param>
		/// <returns>System.Data.DataTable</returns>
		public static DataTable LinqToDataTable( DataContext ctx, IQueryable query )
		{
			if( query == null )
			{
				throw new ArgumentNullException( "Query object cannot be null" );
			}

			IDbCommand cmd = ctx.GetCommand( query );
			SqlDataAdapter adapter = new SqlDataAdapter();
			adapter.SelectCommand = ( SqlCommand )cmd;
			DataTable dt = new DataTable( "dTable" );

			try
			{
				cmd.Connection.Open();
				adapter.FillSchema( dt, SchemaType.Source );
				adapter.Fill( dt );
			}
			finally
			{
				cmd.Connection.Close();
			}

			return dt;
		}

		public static DataView LinqToDataView( DataContext ctx, object query )
		{
			DataTable dt = LinqToDataTable( ctx, query as IQueryable );

			if( dt == null )
			{
				return null;
			}
			else
			{
				//DataView dv = new DataView( dt );
				return ( new DataView( dt ) );
			}
		}

		/// <summary>
		/// Converts a Linq IEnumerable or IQueryable list to a DataTable,
		/// without the need to re-query the data
		/// </summary>

		public static DataTable LinqToDataTable<T>( IEnumerable<T> varlist )
		{
			DataTable dtReturn = new DataTable();
			// column names
			PropertyInfo[] oProps = null;
			if( varlist == null ) return dtReturn;

			foreach( T rec in varlist )
			{
				// Create table only first time through
				if( oProps == null )
				{
					// Use reflection to get property names,
					// then iterate through and create columns
					oProps = ( ( Type )rec.GetType() ).GetProperties();
					foreach( PropertyInfo pi in oProps )
					{
						Type colType = pi.PropertyType;

						if( ( colType.IsGenericType ) && ( colType.GetGenericTypeDefinition() == typeof( Nullable<> ) ) )
						{
							colType = colType.GetGenericArguments()[ 0 ];
						}
						dtReturn.Columns.Add( new DataColumn( pi.Name, colType ) );
					}
				}
				// Extract the data from the List
				// and insert into the DataTable
				DataRow dr = dtReturn.NewRow();
				foreach( PropertyInfo pi in oProps )
				{
					dr[ pi.Name ] = pi.GetValue( rec, null ) == null ? DBNull.Value : pi.GetValue
					( rec, null );
				}
				dtReturn.Rows.Add( dr );
			}
			return dtReturn;
		}

		#region TransactionScope Helper Methods

		/// <summary>
		/// Better default Transaction options
		/// </summary>
		public static TransactionScope CreateTransactionScope( TransactionScopeOption tso = TransactionScopeOption.Required )
		{
			// http://blogs.msdn.com/b/dbrowne/archive/2010/05/21/using-new-transactionscope-considered-harmful.aspx

			TransactionOptions transactionOptions = new TransactionOptions();
			transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
			transactionOptions.Timeout = TransactionManager.MaximumTimeout;
			return new TransactionScope( tso, transactionOptions );
		}

		public static void ExecuteWithDeadlockRetry
		(
			Action action,
			Int32 maxRetries = 5,
			Boolean useTransaction = true,
			Boolean waitRandom = true )
		{
			Int32 tryCount = 0;

			// If use transaction is true and there is an existing ambient transaction
			// (means we're in a transaction already) then it will not do any good to
			// attempt any retries, so set max retry limit to 1

			if( useTransaction && Transaction.Current != null )
			{
				maxRetries = 1;
			}

			do
			{
				try
				{
					// increment the try count...
					tryCount++;

					if( useTransaction )
					{
						// execute the action inside a transaction...
						using( TransactionScope transactionScope =
							CreateTransactionScope( TransactionScopeOption.RequiresNew ) )
						{
							action();
							transactionScope.Complete();
						}
					}
					else
					{
						action();
					}

					// If here, execution was successful, so we can return...
					return;
				}
				catch( SqlException sqlException ) when( sqlException.Number == 1205 && tryCount < maxRetries )
				{
					Common.DrawWarning( "DEADLOCK: Retrying action" );

					if( waitRandom )
					{
						Thread.Sleep( Randomizer.Int32( 5000 ) );
					}
				}
			}
			while( tryCount <= maxRetries );
		}

		#endregion
	}

	public static class PredicateBuilder
	{
		public static Expression<Func<T, Boolean>> True<T>()
		{
			return f => true;
		}

		public static Expression<Func<T, Boolean>> False<T>()
		{
			return f => false;
		}

		public static Expression<Func<T, Boolean>> Or<T>( this Expression<Func<T, Boolean>> expr1,
															Expression<Func<T, Boolean>> expr2 )
		{
			var invokedExpr = Expression.Invoke( expr2, expr1.Parameters.Cast<Expression>() );
			return Expression.Lambda<Func<T, Boolean>>
				  ( Expression.OrElse( expr1.Body, invokedExpr ), expr1.Parameters );
		}

		public static Expression<Func<T, Boolean>> And<T>( this Expression<Func<T, Boolean>> expr1,
															 Expression<Func<T, Boolean>> expr2 )
		{
			var invokedExpr = Expression.Invoke( expr2, expr1.Parameters.Cast<Expression>() );
			return Expression.Lambda<Func<T, Boolean>>
				  ( Expression.AndAlso( expr1.Body, invokedExpr ), expr1.Parameters );
		}
	}

	public struct NonNull<T> where T : class
	{
		private readonly T _value;

		public T Value { get { return _value; } }

		[Obsolete( "The default parameterless constructor has been disabled please use NonNull<T>(T instance) constructor instead.", true )]
		public NonNull( object dummy )
		{
			throw new NotSupportedException( "The default parameterless constructor has been disabled please use NonNull<T>(T instance) constructor instead." );
		}

		public NonNull( T instance )
		{
			if( instance == null )
			{
				throw new ArgumentNullException( "instance", "The \"instance\" parameter must not be null." );
			}
			_value = instance;
		}

		public override Int32 GetHashCode()
		{
			return _value.GetHashCode();
		}

		public override Boolean Equals( object obj )
		{
			return _value.Equals( obj );
		}

		public static Boolean operator ==( NonNull<T> valueA, NonNull<T> valueB )
		{
			return valueA._value == valueB._value;
		}

		public static Boolean operator !=( NonNull<T> valueA, NonNull<T> valueB )
		{
			return valueA._value != valueB._value;
		}

		public static implicit operator NonNull<T>( T value )
		{
			return new NonNull<T>( value );
		}

		public static explicit operator T( NonNull<T> value )
		{
			return value._value;
		}
	}
}