using System;

namespace Tamasi.Shared.Framework
{
	public static class Constants
	{
		public const Int32 CONSOLE_PROGRESS_OBJ_PER_DOT = 250;

		/// <summary>
		/// Intended for the CommandTimeout property, this is 3 hours in seconds
		/// </summary>
		public const Int32 LONG_QUERY_DB_TIMEOUT = 9600;

		public const char NEW_LINE = '\n';

		public const Int16 MAX_PATH = 260;

		public const Int32 SHORT_PAUSE = 1000;

		public const Int32 SYMBOLIC_LINK_FLAG_DIRECTORY = 1;

#if (DEBUG)
		public const Boolean IS_DEBUG_MODE = true;
#else
		public const Boolean IS_DEBUG_MODE = false;
#endif

	}

	public sealed class ErrorConstants
	{
		private ErrorConstants()
		{
			// private constructor to prevent compiler
			// from auto-adding a default constructor
		}

		private const Int32 ERROR_MASK = ( 1 << 31 ); //0x8000_0000;
		public const Int32 S_OK = 0;
		public const Int32 E_NO_ATTRIBUTE = ERROR_MASK | 1;   // Attempt was made to access an XML attribute that doesn't exist
		public const Int32 E_BAD_FILE_ACCESS = ERROR_MASK | 2;   // An error occurred while accessing a file
		public const Int32 E_FAIL = ERROR_MASK | 3;   // Some operation failed. Do we need a less generic error?
		public const Int32 E_BAD_FIELD = ERROR_MASK | 4;   // An error occurred while accessing a field of some sort (Product Studio or XML--maybe make 2 separate errors?)
		public const Int32 E_READ_ONLY = ERROR_MASK | 5;   // An object (probably a PS bug) has not been opened for editing
		public const Int32 E_NO_FIELD = ERROR_MASK | 6;   // An attempt was made to access a PS field that doesn't exist
		public const Int32 E_QUERY_FAILED = ERROR_MASK | 7;   // A PS query failed
		public const Int32 E_CONNECT_FAILED = ERROR_MASK | 8;   // Could not connect to Product Studio
		public const Int32 E_PS_PATH_READ_FAILED = ERROR_MASK | 9;   // Could not determine the Product Studio path for a bug
		public const Int32 E_CONFIG_FAILED = ERROR_MASK | 10;  // An error occurred while processing the configuration
		public const Int32 S_CONFIG_OPTION = 11;               // An invalid configuration option was specified. Not an error, just a warning.
		public const Int32 E_INTERNAL_ERROR = 12;               // An error was caused by bad programming. The user can't fix or work around the problem.
		public const Int32 E_NO_VARIABLE = 13;               // The specified variable does not exist

		static public Boolean SUCCEEDED( Int32 error ) { return ( ( error & ERROR_MASK ) == 0 ); }
	};
}
