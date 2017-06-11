using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace WeddingData
{
	public partial class WeddingContext
	{
		#region Fields and Constructors

		private static SqlConnectionStringBuilder builder = null;
		private static string connectionString = null;
		private static string databaseServer = null;
		private static string databaseName = null;
		private static bool hydrated = false;

		#endregion
		
		public static WeddingContext New()
		{

			WeddingContext context = new WeddingContext( ConnectionString );

			// Options:  http://msdn.microsoft.com/en-us/library/system.data.linq.datacontext.aspx
			
			return context;
		}

		public static string ConnectionString
		{
			get
			{
				if( connectionString == null )
				{
					connectionString = ConfigurationManager.ConnectionStrings[ "Wedding" ]
												.ConnectionString;

					if( string.IsNullOrEmpty( connectionString ) )
					{
						throw new ApplicationException( "Invalid Wedding ConnectionString" );
					}
				}

				return connectionString;
			}
		}

		public static string DatabaseServer
		{
			get
			{
				if( !hydrated )
				{
					Hydrate();
				}

				Debug.Assert( !string.IsNullOrEmpty( databaseServer ) );
				return databaseServer;
			}
		}

		public static string DatabaseName
		{
			get
			{
				if( !hydrated )
				{
					Hydrate();
				}

				Debug.Assert( !string.IsNullOrEmpty( databaseName ) );
				return databaseName;
			}
		}

		/// <summary>
		/// Set the connection manually, e.g., from the a web site or other startup that
		/// can't use the default ConfigurationManager
		/// </summary>
		/// <param name="connectionString"></param>
		public static void Initialize( string connString )
		{
			connectionString = connString;
			Hydrate();
		}

		#region Privates

		/// <summary>
		/// Populate the static properties and test the connection
		/// </summary>
		private static void Hydrate()
		{
			builder = new SqlConnectionStringBuilder()
			{
				ConnectionString = ConnectionString
			};

			databaseServer = builder.DataSource;
			databaseName = builder.InitialCatalog;

			if( !New().DatabaseExists() )
			{
				throw new ApplicationException( String.Format
				(
					"Cannot connect to [{0}].[{1}] db with '{2}'",
					databaseServer,
					databaseName,
					connectionString
				) );
				// TODO Pri 1 dispose
			}

			hydrated = true;
		}

		#endregion
	}
}
