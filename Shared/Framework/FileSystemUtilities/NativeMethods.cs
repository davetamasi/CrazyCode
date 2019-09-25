using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	[Obsolete]
	internal class NativeMethods
	{
		[StructLayout( LayoutKind.Sequential )]
		internal class FileTime
		{
			internal FileTime( FILETIME myTime )
			{
				highValue = myTime.dwHighDateTime;
				lowValue = myTime.dwLowDateTime;
			}
			public Int32 lowValue;
			public Int32 highValue;
		}

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern IntPtr FindFirstFileW( string lpFileName, out WIN32_FIND_DATA lpFindFileData );

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern Boolean FindNextFileW( IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData );

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern SafeFileHandle CreateFileW(
			string lpFileName,
			FileAccessEx dwDesiredAccess,
			FileShareEx dwShareMode,
			IntPtr SecurityAttributes,
			CreationDisposition dwCreationDisposition,
			FileAttributes dwFlagsAndAttributes,
			IntPtr hTemplateFile
		);

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		internal static extern Boolean GetFileAttributesExW(
			string lpFileName,
			GET_FILEEX_INFO_LEVELS fInfoLevelId,
			out WIN32_FILE_ATTRIBUTE_DATA fileData
		);

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		internal static extern Boolean SetFileTime(
			SafeFileHandle hFile,
			FileTime lpCreationTime,
			FileTime lpLastAccessTime,
			FileTime lpLastWriteTime );

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		internal static extern Boolean DeleteFileW( string lpFileName );

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		internal static extern Boolean CopyFileW( string lpExistingFileName, string lpNewFileName, Boolean bFailIfExists );

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		internal static extern Boolean MoveFileW( string src, string dst );

		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode )]
		internal static extern Boolean SetFileAttributesW( string lpFileName, FileAttributes dwFileAttributes );

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern Boolean CreateDirectoryW(
			string lpPathName,
			IntPtr lpSecurityAttributes
		);

		[DllImport( "kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true )]
		internal static extern Boolean RemoveDirectoryW( string lpPathName );

		[DllImport( "kernel32.dll" )]
		internal static extern Boolean FindClose( IntPtr hFindFile );

		[Obsolete]
		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		internal static extern Int32 GetShortPathName(
			String lpszLongPath,                // input string
			StringBuilder lpszShortPath,        // output string
			Int32 cchBuffer );                   // StringBuilder.Capacity

		[Obsolete]
		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		internal static extern Boolean MoveFileEx(
			string lpExistingFileName,
			string lpNewFileName,
			Int32 dwFlags
		);

		[Obsolete]
		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		internal static extern Boolean MoveFile(
			string lpExistingFileName,
			string lpNewFileName
		);

		[Obsolete]
		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		internal static extern UInt32 GetFullPathName(
			string lpFileName,
			UInt32 nBufferLength,
			StringBuilder lpBuffer,
			IntPtr lpFilePart
		);

		[Obsolete]
		[DllImport( "kernel32.dll", SetLastError = true, CharSet = CharSet.Auto )]
		internal static extern Boolean CreateSymbolicLink(
			string lpSymlinkFileName,
			string lpTargetFileName,
			Int32 dwFlags
		);
		
		[StructLayout( LayoutKind.Sequential )]
		internal struct WIN32_FILE_ATTRIBUTE_DATA
		{
			public FileAttributes dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public UInt32 nFileSizeHigh;
			public UInt32 nFileSizeLow;
		}

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )]
		internal struct WIN32_FIND_DATA
		{
			public FileAttributes dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public UInt32 nFileSizeHigh;
			public UInt32 nFileSizeLow;
			public UInt32 dwReserved0;
			public UInt32 dwReserved1;
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
			public string cFileName;
			[MarshalAs( UnmanagedType.ByValTStr, SizeConst = 14 )]
			public string cAlternateFileName;
		}
	}
}
