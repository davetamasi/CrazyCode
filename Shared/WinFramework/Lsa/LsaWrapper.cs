using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Tamasi.Shared.WinFramework.Lsa
{
	/// <summary>
	/// This class is used to grant "Log on as a service", "Log on as a batchjob", "Log on localy"
	/// etc. to a user.
	/// </summary>
	internal sealed class LsaWrapper : IDisposable
	{
		[StructLayout( LayoutKind.Sequential )]
		private struct LSA_TRUST_INFORMATION
		{
			internal LSA_UNICODE_STRING Name;
			internal IntPtr Sid;
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct LSA_TRANSLATED_SID2
		{
			internal SidNameUse Use;
			internal IntPtr Sid;
			internal Int32 DomainIndex;
			private UInt32 Flags;
		}

		[StructLayout( LayoutKind.Sequential )]
		private struct LSA_REFERENCED_DOMAIN_LIST
		{
			internal UInt32 Entries;
			internal LSA_TRUST_INFORMATION Domains;
		}

		private const UInt32 STATUS_ACCESS_DENIED = 0xc0000022;
		private const UInt32 STATUS_INSUFFICIENT_RESOURCES = 0xc000009a;
		private const UInt32 STATUS_NO_MEMORY = 0xc0000017;

		private IntPtr lsaHandle;

		internal LsaWrapper()
			: this( null )
		{ }

		// // local system if systemName is null
		internal LsaWrapper( string systemName )
		{
			LSA_OBJECT_ATTRIBUTES lsaAttr;
			lsaAttr.RootDirectory = IntPtr.Zero;
			lsaAttr.ObjectName = IntPtr.Zero;
			lsaAttr.Attributes = 0;
			lsaAttr.SecurityDescriptor = IntPtr.Zero;
			lsaAttr.SecurityQualityOfService = IntPtr.Zero;
			lsaAttr.Length = Marshal.SizeOf( typeof( LSA_OBJECT_ATTRIBUTES ) );
			lsaHandle = IntPtr.Zero;
			LSA_UNICODE_STRING[] system = null;
			if( systemName != null )
			{
				system = new LSA_UNICODE_STRING[ 1 ];
				system[ 0 ] = InitLsaString( systemName );
			}

			UInt32 ret = Win32Sec.LsaOpenPolicy( system, ref lsaAttr,
			( Int32 )Access.POLICY_ALL_ACCESS, out lsaHandle );
			if( ret == 0 )
				return;
			if( ret == STATUS_ACCESS_DENIED )
			{
				throw new UnauthorizedAccessException();
			}
			if( ( ret == STATUS_INSUFFICIENT_RESOURCES ) || ( ret == STATUS_NO_MEMORY ) )
			{
				throw new OutOfMemoryException();
			}
			throw new Win32Exception( Win32Sec.LsaNtStatusToWinError( ( Int32 )ret ) );
		}

		public void AddPrivileges( string account, string privilege )
		{
			IntPtr pSid = GetSIDInformation( account );
			LSA_UNICODE_STRING[] privileges = new LSA_UNICODE_STRING[ 1 ];
			privileges[ 0 ] = InitLsaString( privilege );
			UInt32 ret = Win32Sec.LsaAddAccountRights( lsaHandle, pSid, privileges, 1 );
			if( ret == 0 )
				return;
			if( ret == STATUS_ACCESS_DENIED )
			{
				throw new UnauthorizedAccessException();
			}
			if( ( ret == STATUS_INSUFFICIENT_RESOURCES ) || ( ret == STATUS_NO_MEMORY ) )
			{
				throw new OutOfMemoryException();
			}
			throw new Win32Exception( Win32Sec.LsaNtStatusToWinError( ( Int32 )ret ) );
		}

		public void Dispose()
		{
			if( lsaHandle != IntPtr.Zero )
			{
				Win32Sec.LsaClose( lsaHandle );
				lsaHandle = IntPtr.Zero;
			}
			GC.SuppressFinalize( this );
		}

		~LsaWrapper()
		{
			Dispose();
		}

		// helper functions

		private IntPtr GetSIDInformation( string account )
		{
			LSA_UNICODE_STRING[] names = new LSA_UNICODE_STRING[ 1 ];
			LSA_TRANSLATED_SID2 lts;
			IntPtr tsids = IntPtr.Zero;
			IntPtr tdom = IntPtr.Zero;
			names[ 0 ] = InitLsaString( account );
			lts.Sid = IntPtr.Zero;
			Console.WriteLine( "String account: {0}", names[ 0 ].Length );
			Int32 ret = Win32Sec.LsaLookupNames2( lsaHandle, 0, 1, names, ref tdom, ref tsids );
			if( ret != 0 )
				throw new Win32Exception( Win32Sec.LsaNtStatusToWinError( ret ) );
			lts = ( LSA_TRANSLATED_SID2 )Marshal.PtrToStructure( tsids,
			typeof( LSA_TRANSLATED_SID2 ) );
			Win32Sec.LsaFreeMemory( tsids );
			Win32Sec.LsaFreeMemory( tdom );
			return lts.Sid;
		}

		private static LSA_UNICODE_STRING InitLsaString( string s )
		{
			// Unicode strings max. 32KB
			if( s.Length > 0x7ffe )
				throw new ArgumentException( "String too Int64" );
			LSA_UNICODE_STRING lus = new LSA_UNICODE_STRING();
			lus.Buffer = s;
			lus.Length = ( UInt16 )( s.Length * sizeof( char ) );
			lus.MaximumLength = ( UInt16 )( lus.Length + sizeof( char ) );
			return lus;
		}
	}
}