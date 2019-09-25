using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	internal static class Constants
	{
		// TODO Pri 2: Put this in config
		internal const string ROBOCOPY_EXE_PATH = @"C:\Windows\system32\robocopy.exe";

		/// <summary>
		/// Copy to path\file.vhd.transferring, then rename remove the .transferring
		/// </summary>
		internal const string COPY_MARKER = ".transferring";

		// We know .NET IO routines blow up on the following characters
		internal static readonly char[] badchars = new char[]
		{
			'\xA0', '\x85', '\xFF', '\x22', '\x2A',
			'\x3C', '\x3E', '\x3F',  '\x7C'
		};
	}
}
