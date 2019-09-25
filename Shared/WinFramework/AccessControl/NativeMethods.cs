using System;
using System.Runtime.InteropServices;

namespace Tamasi.Shared.WinFramework.AccessControl
{
	internal static class NativeMethods
	{
		[DllImport( "Advapi32.dll", CharSet = CharSet.Unicode )]
		internal static extern Boolean ConvertStringSidToSid( [MarshalAs( UnmanagedType.LPWStr )] string StringSid, out IntPtr Sid );

		[DllImport( "Advapi32.dll" )]
		internal static extern Boolean ConvertSidToStringSid( IntPtr Sid, out IntPtr StringSid );

		[DllImport( "Advapi32.dll" )]
		internal static extern UInt32 GetSidLengthRequired( UInt16 nSubAuthorityCount );

		[DllImport( "Advapi32.dll" )]
		internal static extern Boolean CreateWellKnownSid( Int32 WellKnownSidType, IntPtr DomainSid, IntPtr pSid, ref UInt32 cbSid );

		[DllImport( "Advapi32.dll" )]
		internal static extern Boolean LookupAccountSid( [MarshalAs( UnmanagedType.LPTStr )]string lpSystemName,
														IntPtr lpSid,
														IntPtr lpName,
														ref UInt32 cchName,
														IntPtr lpReferencedDomainName,
														ref UInt32 cchReferencedDomainName,
														out SID_NAME_USE peUse );

		[DllImport( "Advapi32.dll" )]
		internal static extern Int32 LsaOpenPolicy( IntPtr SystemName,
												ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
												ACCESS_MASK DesiredAccess,
												out IntPtr PolicyHandle );

		[DllImport( "Advapi32.dll" )]
		internal static extern UInt32 LsaNtStatusToWinError( Int32 Status );

		[DllImport( "Advapi32.dll" )]
		internal static extern Int32 LsaLookupNames2( IntPtr PolicyHandle,
													UInt32 Flags,
													UInt32 Count,
													LSA_UNICODE_STRING[] Names,
													out IntPtr ReferencedDomains,
													out IntPtr Sids );

		[DllImport( "Advapi32.dll" )]
		internal static extern Int32 LsaClose( IntPtr ObjectHandle );

		[DllImport( "Advapi32.dll" )]
		internal static extern Int32 LsaFreeMemory( IntPtr Buffer );

		[DllImport( "Kernel32.dll" )]
		internal static extern IntPtr LocalFree( IntPtr hMem );

		[DllImport( "kernel32.dll" )]
		internal static extern UInt32 GetLastError();

		[DllImport( "kernel32.dll", CharSet = CharSet.Auto )]
		internal static extern UInt32 FormatMessage
		(
			UInt32 dwFlags,
			IntPtr lpSource,
			UInt32 dwMessageId,
			UInt32 dwLanguageId,
			[MarshalAs( UnmanagedType.LPTStr )] ref string lpBuffer,
			Int32 nSize,
			IntPtr[] Arguments
		);

		internal enum SID_NAME_USE
		{
			SidTypeUser = 1,
			SidTypeGroup,
			SidTypeDomain,
			SidTypeAlias,
			SidTypeWellKnownGroup,
			SidTypeDeletedAccount,
			SidTypeInvalid,
			SidTypeUnknown,
			SidTypeComputer
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		internal struct LSA_UNICODE_STRING
		{
			public UInt16 Length;
			public UInt16 MaxLength;

			[MarshalAs( UnmanagedType.LPWStr )]
			public string Buffer;
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct LSA_TRANSLATED_SID2
		{
			public SID_NAME_USE Use;
			public IntPtr Sid;
			public Int32 DomainIndex;
			public UInt32 Flags;
		}

		[StructLayout( LayoutKind.Sequential )]
		internal struct LSA_OBJECT_ATTRIBUTES
		{
			public UInt32 Length;
			public IntPtr RootDirectory;
			public IntPtr ObjectName;
			public UInt32 Attributes;
			public IntPtr SecurityDescriptor;
			public IntPtr SecurityQualityOfService;
		}

		[Flags]
		internal enum ACCESS_MASK
		{
			POLICY_VIEW_LOCAL_INFORMATION = 0x0001,
			POLICY_VIEW_AUDIT_INFORMATION = 0x0002,
			POLICY_GET_PRIVATE_INFORMATION = 0x0004,
			POLICY_TRUST_ADMIN = 0x0008,
			POLICY_CREATE_ACCOUNT = 0x0010,
			POLICY_CREATE_SECRET = 0x0020,
			POLICY_CREATE_PRIVILEGE = 0x0040,
			POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x0080,
			POLICY_SET_AUDIT_REQUIREMENTS = 0x0100,
			POLICY_AUDIT_LOG_ADMIN = 0x0200,
			POLICY_SERVER_ADMIN = 0x0400,
			POLICY_LOOKUP_NAMES = 0x0800
		}
	}
}