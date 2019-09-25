using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Tamasi.Shared.WinFramework.Lsa
{
	using LSA_HANDLE = IntPtr;

	internal sealed class Win32Sec
	{
		[DllImport( "advapi32", CharSet = CharSet.Unicode, SetLastError = true ),
		SuppressUnmanagedCodeSecurityAttribute]
		internal static extern UInt32 LsaOpenPolicy
		(
			LSA_UNICODE_STRING[] SystemName,
			ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
			Int32 AccessMask,
			out IntPtr PolicyHandle
		);

		[DllImport( "advapi32", CharSet = CharSet.Unicode, SetLastError = true ),
		SuppressUnmanagedCodeSecurityAttribute]
		internal static extern UInt32 LsaAddAccountRights
		(
			LSA_HANDLE PolicyHandle,
			IntPtr pSID,
			LSA_UNICODE_STRING[] UserRights,
			Int32 CountOfRights
		);

		[DllImport( "advapi32", CharSet = CharSet.Unicode, SetLastError = true ),
		SuppressUnmanagedCodeSecurityAttribute]
		internal static extern Int32 LsaLookupNames2
		(
			LSA_HANDLE PolicyHandle,
			UInt32 Flags,
			UInt32 Count,
			LSA_UNICODE_STRING[] Names,
			ref IntPtr ReferencedDomains,
			ref IntPtr Sids
		);

		[DllImport( "advapi32" )]
		internal static extern Int32 LsaNtStatusToWinError( Int32 NTSTATUS );

		[DllImport( "advapi32" )]
		internal static extern Int32 LsaClose( IntPtr PolicyHandle );

		[DllImport( "advapi32" )]
		internal static extern Int32 LsaFreeMemory( IntPtr Buffer );
	}
}