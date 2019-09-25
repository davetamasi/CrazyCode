using System;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	public enum DriveLetter
	{
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K
	}
	
	public enum GET_FILEEX_INFO_LEVELS
	{
		GetFileExInfoStandard,
		GetFileExMaxInfoLevel
	}

	public enum FIND_DATA_TYPE
	{
		Directory,
		File
	}

	[Flags]
	public enum FileAccessEx : uint
	{
		GenericRead = 0x80000000,
		GenericWrite = 0x40000000,
		GenericExecute = 0x20000000,
		GenericAll = 0x10000000
	}

	[Flags]
	public enum FileShareEx : uint
	{
		None = 0x00000000,
		Read = 0x00000001,
		Write = 0x00000002,
		Delete = 0x00000004
	}

	public enum CreationDisposition : uint
	{
		New = 1,
		CreateAlways = 2,
		OpenExisting = 3,
		OpenAlways = 4,
		TruncateExisting = 5
	}

	[Flags]
	public enum RobocopyExit
	{
		/// <summary>
		/// Serious error. Robocopy did not copy any files. Either a usage error or an error due to insufficient access privileges on the source or destination directories.
		/// </summary>
		SeriousError = 16,

		/// <summary>
		/// Some files or directories could not be copied (copy errors occurred and the retry limit was exceeded).  Check these errors further.
		/// </summary>
		SomeErrors = 8,

		/// <summary>
		/// Some Mismatched files or directories were detected.  Examine the output log. Some housekeeping may be needed.
		/// </summary>
		MismatchedObjectsDetected = 4,

		/// <summary>
		///  Some Extra files or directories were detected. Examine the output log for details. 
		/// </summary>
		ExtraObjectsDetected = 2,

		/// <summary>
		/// One or more files were copied successfully (that is, new files have arrived).
		/// </summary>
		Success = 1,

		/// <summary>
		/// No errors occurred, and no copying was done. The source and destination directory trees are completely synchronized. 
		/// </summary>
		NoWorkDone = 0,
	}
}
