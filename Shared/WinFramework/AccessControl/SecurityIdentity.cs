using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace Tamasi.Shared.WinFramework.AccessControl
{
	/// <summary>
	/// Security Identity
	/// </summary>
	/// <remarks>
	/// The SecurityIdentity class is a read only representation of a SID. The class has no public
	/// constructors, instead use the static SecurityIdentityFrom* methods to instantiate it.
	/// </remarks>
	public sealed class SecurityIdentity
	{
		#region Fields and Constructors

		private string name;
		private string sid;
		private WellKnownSidType wellKnownSidType;

		/// <summary>
		/// Creates Security Identity from a SID string or an object name
		/// </summary>
		/// <param name="value">A SID string (Format: S-1-1-...) or an object name (e.g. DOMAIN\AccountName)</param>
		public SecurityIdentity( string value )
		{
			this.wellKnownSidType = ( WellKnownSidType ) - 1;

			if( value.StartsWith( "S-" ) )
			{
				//SID string
				SetFromSid( value );
			}
			else
			{
				//SID string
				SetFromName( value );
			}
		}

		/// <summary>
		/// Creates a Security Identity for a well known SID (such as LOCAL SYSTEM)
		/// </summary>
		/// <param name="sidType">The type of well known SID</param>
		public SecurityIdentity( WellKnownSidType sidType )
		{
			this.wellKnownSidType = ( WellKnownSidType ) - 1;

			SetFromWellKnownSidType( sidType );
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets the name of to security object represented by the SID
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}

		/// <summary>
		/// Gets the SID string of the security object
		/// </summary>
		public string SID
		{
			get { return this.sid; }
		}

		/// <summary>
		/// Gets a value indicating whether or not the SID is a well known SID or not
		/// </summary>
		public Boolean IsWellKnownSid
		{
			get { return ( Int32 )this.wellKnownSidType != -1; }
		}

		/// <summary>
		/// Gets the SDDL abbreviation of well known SID
		/// </summary>
		public string WellKnownSidAbbreviations
		{
			get
			{
				if( this.IsWellKnownSid && !String.IsNullOrEmpty( Constants.WellKnownSIDAbbreviations[ ( Int32 )this.wellKnownSidType ] ) )
				{
					return Constants.WellKnownSIDAbbreviations[ ( Int32 )this.wellKnownSidType ];
				}

				return null;
			}
		}

		/// <summary>
		/// Gets the type of well known SID
		/// </summary>
		public WellKnownSidType WellKnownSidType
		{
			get { return this.wellKnownSidType; }
		}

		#endregion

		#region Methods

		private void SetFromSid( string sid )
		{
			if( sid == null )
			{
				throw new ArgumentNullException( nameof( sid ) );
			}

			if( sid.Length == 0 )
			{
				throw new ArgumentException( "Argument 'sid' cannot be the empty string.", "sid" );
			}

			this.sid = sid;

			// Check if the SID is a well known SID
			this.wellKnownSidType = ( WellKnownSidType )Array.IndexOf<string>( Constants.WellKnownSIDs, sid );

			IntPtr sidStruct;

			// Convert the SID string to a SID structure
			if( !NativeMethods.ConvertStringSidToSid( sid, out sidStruct ) )
			{
				throw new ExternalException( String.Format
				(
					"Error Converting SID String to SID Structur: {0}",
					GetErrorMessage( NativeMethods.GetLastError() ) )
				);
			}

			try
			{
				UInt32 nameLen = 0;
				UInt32 domainLen = 0;

				NativeMethods.SID_NAME_USE nameUse;

				// Get the lengths of the object and domain names
				NativeMethods.LookupAccountSid( null, sidStruct, IntPtr.Zero, ref nameLen, IntPtr.Zero, ref domainLen, out nameUse );

				if( nameLen != 0 )
				{
					IntPtr accountName = Marshal.AllocHGlobal( ( IntPtr )nameLen );
					IntPtr domainName = domainLen > 0 ? Marshal.AllocHGlobal( ( IntPtr )domainLen ) : IntPtr.Zero;

					try
					{
						// Get the object and domain names
						if( !NativeMethods.LookupAccountSid( null, sidStruct, accountName, ref nameLen, domainName, ref domainLen, out nameUse ) )
						{
							throw new ExternalException( "Unable to Find SID" );
						}

						// Marshal and store the object name
						this.name = String.Format( "{0}{1}{2}", domainLen > 1 ? Marshal.PtrToStringAnsi( domainName ) : "", domainLen > 1 ? "\\" : "", Marshal.PtrToStringAnsi( accountName ) );
					}
					finally
					{
						if( accountName != IntPtr.Zero )
						{
							Marshal.FreeHGlobal( accountName );
						}
						if( domainName != IntPtr.Zero )
						{
							Marshal.FreeHGlobal( domainName );
						}
					}
				}
			}
			finally
			{
				if( sidStruct != IntPtr.Zero )
				{
					NativeMethods.LocalFree( sidStruct );
				}
			}
		}

		private void SetFromName( string name )
		{
			if( name == null )
			{
				throw new ArgumentNullException( nameof( name ) );
			}

			if( name.Length == 0 )
			{
				throw new ArgumentException( "Argument 'name' cannot be the empty string.", "name" );
			}

			NativeMethods.LSA_OBJECT_ATTRIBUTES attribs = new NativeMethods.LSA_OBJECT_ATTRIBUTES();
			attribs.Attributes = 0;
			attribs.ObjectName = IntPtr.Zero;
			attribs.RootDirectory = IntPtr.Zero;
			attribs.SecurityDescriptor = IntPtr.Zero;
			attribs.SecurityQualityOfService = IntPtr.Zero;
			attribs.Length = ( UInt32 )Marshal.SizeOf( attribs );

			IntPtr handle;

			Int32 status = NativeMethods.LsaOpenPolicy( IntPtr.Zero, ref attribs, NativeMethods.ACCESS_MASK.POLICY_LOOKUP_NAMES, out handle );

			if( status != 0 )
			{
				throw new ExternalException( "Unable to Find Object: " + GetErrorMessage( NativeMethods.LsaNtStatusToWinError( status ) ) );
			}

			try
			{
				NativeMethods.LSA_UNICODE_STRING nameString = new NativeMethods.LSA_UNICODE_STRING();
				nameString.Buffer = name;
				nameString.Length = ( UInt16 )( name.Length * UnicodeEncoding.CharSize );
				nameString.MaxLength = ( UInt16 )( name.Length * UnicodeEncoding.CharSize + UnicodeEncoding.CharSize );

				IntPtr domains;
				IntPtr sids;

				status = NativeMethods.LsaLookupNames2( handle, 0, 1, new NativeMethods.LSA_UNICODE_STRING[] { nameString }, out domains, out sids );

				if( status != 0 )
				{
					throw new ExternalException( "Unable to Find Object: " + GetErrorMessage( NativeMethods.LsaNtStatusToWinError( status ) ) );
				}

				try
				{
					NativeMethods.LSA_TRANSLATED_SID2 lsaSid = ( NativeMethods.LSA_TRANSLATED_SID2 )Marshal.PtrToStructure
					(
						sids,
						typeof( NativeMethods.LSA_TRANSLATED_SID2 )
					);

					IntPtr sidStruct = lsaSid.Sid;

					IntPtr sidString = IntPtr.Zero;

					// Get the SID string
					if( !NativeMethods.ConvertSidToStringSid( sidStruct, out sidString ) )
					{
						throw new ExternalException( "Unable to Find Object: " + GetErrorMessage( NativeMethods.GetLastError() ) );
					}

					try
					{
						// Marshal and store the SID string
						this.sid = Marshal.PtrToStringAnsi( sidString );
					}
					finally
					{
						if( sidString != IntPtr.Zero )
						{
							NativeMethods.LocalFree( sidString );
						}
					}

					// Check if the SID is a well known SID
					this.wellKnownSidType = ( WellKnownSidType )Array.IndexOf<string>( Constants.WellKnownSIDs, this.sid );

					NativeMethods.SID_NAME_USE nameUse;

					UInt32 nameLen = 0;
					UInt32 domainLen = 0;

					// Get the lengths for the object and domain names
					NativeMethods.LookupAccountSid( null, sidStruct, IntPtr.Zero, ref nameLen, IntPtr.Zero, ref domainLen, out nameUse );

					if( nameLen == 0 )
					{
						throw new ExternalException( "Unable to Find SID: " + GetErrorMessage( NativeMethods.GetLastError() ) );
					}

					IntPtr accountName = Marshal.AllocHGlobal( ( IntPtr )nameLen );
					IntPtr domainName = domainLen > 0 ? Marshal.AllocHGlobal( ( IntPtr )domainLen ) : IntPtr.Zero;

					try
					{
						// Get the object and domain names
						if( !NativeMethods.LookupAccountSid( null, sidStruct, accountName, ref nameLen, domainName, ref domainLen, out nameUse ) )
						{
							throw new ExternalException( "Unable to Find SID: " + GetErrorMessage( NativeMethods.GetLastError() ) );
						}

						// Marshal and store the object name
						this.name = String.Format( "{0}{1}{2}", domainLen > 1 ? Marshal.PtrToStringAnsi( domainName ) : "", domainLen > 1 ? "\\" : "", Marshal.PtrToStringAnsi( accountName ) );
					}
					finally
					{
						if( accountName != IntPtr.Zero )
						{
							Marshal.FreeHGlobal( accountName );
						}
						if( domainName != IntPtr.Zero )
						{
							Marshal.FreeHGlobal( domainName );
						}
					}
				}
				finally
				{
					if( domains != IntPtr.Zero )
					{
						NativeMethods.LsaFreeMemory( domains );
					}
					if( sids != IntPtr.Zero )
					{
						NativeMethods.LsaFreeMemory( sids );
					}
				}
			}
			finally
			{
				if( handle != IntPtr.Zero )
				{
					NativeMethods.LsaClose( handle );
				}
			}
		}

		private void SetFromWellKnownSidType( WellKnownSidType sidType )
		{
			if( ( Int32 )sidType == -1 )
			{
				throw new ExternalException( "Unable to Get Well Known SID" );
			}

			this.wellKnownSidType = sidType;

			// Get the size required for the SID
			UInt32 size = NativeMethods.GetSidLengthRequired( Constants.SID_MAX_SUB_AUTHORITIES );

			IntPtr sidStruct = Marshal.AllocHGlobal( ( IntPtr )size );

			try
			{
				// Get the SID struct from the well known SID type
				if( !NativeMethods.CreateWellKnownSid( ( Int32 )sidType, IntPtr.Zero, sidStruct, ref size ) )
				{
					throw new ExternalException( "Unable to Get Well Known SID" );
				}

				IntPtr sidString = IntPtr.Zero;

				// Convert the SID structure to a SID string
				NativeMethods.ConvertSidToStringSid( sidStruct, out sidString );

				try
				{
					// Marshal and store the SID string
					this.sid = Marshal.PtrToStringAnsi( sidString );
				}
				finally
				{
					if( sidString != IntPtr.Zero )
					{
						NativeMethods.LocalFree( sidString );
					}
				}

				UInt32 nameLen = 0;
				UInt32 domainLen = 0;

				NativeMethods.SID_NAME_USE nameUse;

				// Get the lengths of the object and domain names
				NativeMethods.LookupAccountSid( null, sidStruct, IntPtr.Zero, ref nameLen, IntPtr.Zero, ref domainLen, out nameUse );

				if( nameLen != 0 )
				{
					IntPtr accountName = Marshal.AllocHGlobal( ( IntPtr )nameLen );
					IntPtr domainName = domainLen > 0 ? Marshal.AllocHGlobal( ( IntPtr )domainLen ) : IntPtr.Zero;

					try
					{
						// Get the object and domain names
						if( !NativeMethods.LookupAccountSid
						(
							null,
							sidStruct,
							accountName,
							ref nameLen,
							domainName,
							ref domainLen,
							out nameUse ) )
						{
							throw new ExternalException( "Unable to Find SID" );
						}

						// Marshal and store the object name
						this.name = String.Format( "{0}{1}{2}", domainLen > 1 ? Marshal.PtrToStringAnsi( domainName ) : "", domainLen > 1 ? "\\" : "", Marshal.PtrToStringAnsi( accountName ) );
					}
					finally
					{
						if( accountName != IntPtr.Zero )
						{
							Marshal.FreeHGlobal( accountName );
						}
						if( domainName != IntPtr.Zero )
						{
							Marshal.FreeHGlobal( domainName );
						}
					}
				}
			}
			finally
			{
				if( sidStruct != IntPtr.Zero )
				{
					Marshal.FreeHGlobal( sidStruct );
				}
			}
		}

		/// <summary>
		/// Gets the Well Known SID Type for an SDDL abbreviation
		/// </summary>
		/// <param name="abbreviation">The SDDL abbreviation</param>
		/// <returns>The Well Known SID Type that corresponds to the abbreviation</returns>
		private static WellKnownSidType GetWellKnownSidTypeFromSddlAbbreviation( string abbreviation )
		{
			if( abbreviation == null )
			{
				throw new ArgumentNullException( nameof( abbreviation ) );
			}

			if( abbreviation == "" )
			{
				throw new ArgumentException( "Argument 'abbreviation' cannot be the empty string.", "abbreviation" );
			}

			return ( WellKnownSidType )Array.IndexOf<string>( Constants.WellKnownSIDAbbreviations, abbreviation );
		}

		private static string GetErrorMessage( UInt32 errorCode )
		{
			UInt32 FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100;
			UInt32 FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200;
			UInt32 FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;

			UInt32 dwFlags = FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS;

			IntPtr source = new IntPtr();

			string msgBuffer = "";

			UInt32 retVal = NativeMethods.FormatMessage( dwFlags, source, errorCode, 0, ref msgBuffer, 512, null );

			return msgBuffer.ToString();
		}

		#endregion

		#region Statics and Overrides

		/// <summary>
		/// Creates a Security Identity from a SID string
		/// </summary>
		/// <param name="sid">
		/// A SID string (Format: S-1-1-...) or well known SID abbreviation (e.g. DA)
		/// </param>
		/// <returns>A populated Security Identity</returns>
		public static SecurityIdentity SecurityIdentityFromSIDorAbbreviation( string value )
		{
			if( value == null )
			{
				throw new ArgumentNullException( nameof( sid ) );
			}

			if( value.Length == 0 )
			{
				throw new ArgumentException( "Argument 'value' cannot be the empty string.", "value" );
			}

			if( !value.StartsWith( "S-" ) )
			{
				// If the string is not a SID string (S-1-n-...) assume it is a SDDL abbreviation
				return new SecurityIdentity( SecurityIdentity.GetWellKnownSidTypeFromSddlAbbreviation( value ) );
			}

			return new SecurityIdentity( value );
		}

		/// <summary>
		/// Creates a Security Identity from an object name (e.g. DOMAIN\AccountName)
		/// </summary>
		/// <param name="name">A security object name (i.e. a Computer, Account, or Group)</param>
		/// <returns>A populated Security Identity</returns>
		public static SecurityIdentity SecurityIdentityFromName( string name )
		{
			return new SecurityIdentity( name );
		}

		public override Boolean Equals( object obj )
		{
			if( obj == null ) return false;
			if( obj is SecurityIdentity )
			{
				SecurityIdentity sd = ( SecurityIdentity )obj;

				return ( String.Compare( this.sid, sd.sid, true ) == 0 );
			}
			else return false;
		}

		public static Boolean operator ==( SecurityIdentity obj1, SecurityIdentity obj2 )
		{
			if( Object.ReferenceEquals( obj1, null ) && Object.ReferenceEquals( obj2, null ) ) return true;
			else if( Object.ReferenceEquals( obj1, null ) || Object.ReferenceEquals( obj2, null ) ) return false;
			return obj1.Equals( obj2 );
		}

		public static Boolean operator !=( SecurityIdentity obj1, SecurityIdentity obj2 )
		{
			if( Object.ReferenceEquals( obj1, null ) && Object.ReferenceEquals( obj2, null ) ) return false;
			else if( Object.ReferenceEquals( obj1, null ) || Object.ReferenceEquals( obj2, null ) ) return true;
			return !obj1.Equals( obj2 );
		}

		public override Int32 GetHashCode()
		{
			return this.sid != null ? this.sid.GetHashCode() : base.GetHashCode();
		}

		/// <summary>
		/// Renders the Security Identity as a SDDL SID string or abbreviation
		/// </summary>
		/// <returns>An SDDL SID string or abbreviation</returns>
		public override string ToString()
		{
			if( this.WellKnownSidAbbreviations == null )
			{
				return this.sid;
			}
			else
			{
				return this.WellKnownSidAbbreviations;
			}
		}

		#endregion
	}
}