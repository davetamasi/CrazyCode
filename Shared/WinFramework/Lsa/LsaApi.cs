using System;
using System.Runtime.InteropServices;

namespace Tamasi.Shared.WinFramework.Lsa
{
	[StructLayout( LayoutKind.Sequential )]
	internal struct LSA_OBJECT_ATTRIBUTES
	{
		internal Int32 Length;
		internal IntPtr RootDirectory;
		internal IntPtr ObjectName;
		internal Int32 Attributes;
		internal IntPtr SecurityDescriptor;
		internal IntPtr SecurityQualityOfService;
	}

	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
	internal struct LSA_UNICODE_STRING
	{
		internal UInt16 Length;
		internal UInt16 MaximumLength;

		[MarshalAs( UnmanagedType.LPWStr )]
		internal String Buffer;
	}

	public class LsaApi
	{
		public static void AddPrivileges( String account, WindowsPrivilege privilege )
		{
			using( LsaWrapper lsaWrapper = new LsaWrapper() )
			{
				lsaWrapper.AddPrivileges( account, privilege.ToString() );
			}
		}
	}
}