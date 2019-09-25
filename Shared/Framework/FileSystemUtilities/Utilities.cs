using System;
using System.Diagnostics;
using System.IO;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	public static class Utilities
	{
		/// <summary>
		/// Recursively deletes the given directory and all its subnodes; will use obliterate.exe if
		/// present, otherwise will revert to CLR methods
		/// </summary>
		/// <param name="directoryPath"></param>
		public static void DeleteTree( string directoryPath )
		{
			Debug.Assert( directoryPath != null );

			if( Directory.Exists( directoryPath ) )
			{
				Common.DrawWarning( "Directory tree to delete, '{0}', does not exist", directoryPath );
				return;
			}

			string oblitPath = Path.Combine( Environment.CurrentDirectory, "Obliterate.exe" );

			if( File.Exists( oblitPath ) )
			{
				Debug.WriteLine( "--> BEGIN obliterate.exe {0}", directoryPath );
				ProcessUtilities.RunProcessWithWait( "Obliterate.exe", directoryPath );
				Debug.WriteLine( "--> END obliterate.exe {0}", directoryPath );
			}
			else
			{
				DirectoryInfo directory = new DirectoryInfo( directoryPath );
				foreach( FileInfo file in directory.GetFiles() )
				{
					file.Delete();
				}
				foreach( DirectoryInfo dir in directory.GetDirectories() )
				{
					dir.Delete( true );
				}
			}

			if( Directory.Exists( directoryPath ) )
			{
				throw new Exception( string.Format( "Unable to delete tree at {0}", directoryPath ) );
			}
		}

		public static Boolean IsLocalPath( string filePath )
		{
			return Path.IsPathRooted( filePath ) && !filePath.StartsWith( @"\" );
		}

		public static Boolean IsFileLocked( VirtualFilePath vfp, FileAccess accessNeeded )
		{
			FileStream stream = null;
			FileInfo file = new FileInfo( vfp.Path );
			Boolean ret = false;

			try
			{
				stream = file.Open( FileMode.Open, accessNeeded, FileShare.None );
			}
			catch( IOException )
			{
				//the file is unavailable because it is:
				//still being written to
				//or being processed by another thread
				//or does not exist (has already been processed)
				ret = true;
			}
			finally
			{
				if( stream != null )
				{
					stream.Close();
				}
			}

			return ret;
		}

		public static string GetOrCreateDirectory( string dirPath )
		{
			if( !Directory.Exists( dirPath ) )
			{
				Directory.CreateDirectory( dirPath );
			}

			return dirPath;
		}

		/// <summary>
		/// Returns the sum of the sizes of all files in a given folder, including subfolders
		/// </summary>
		/// <param name="FolderPath"></param>
		/// <returns></returns>
		public static Int64 GetDirectorySize( string FolderPath )
		{
			DirectoryInfo di = new DirectoryInfo( FolderPath );

			if( !di.Exists )
			{
				return 0;
			}

			return GetDirectorySize( di, 0 );
		}

		/// <summary>
		/// Recursive worker
		/// </summary>
		/// <param name="Dir"></param>
		/// <param name="runningTotal"></param>
		/// <returns>Size of the dir in bytes</returns>
		private static Int64 GetDirectorySize( DirectoryInfo Dir, Int64 runningTotal )
		{
			FileInfo[] Files = Dir.GetFiles();
			foreach( FileInfo fi in Files )
			{
				runningTotal += ( Int64 )fi.Length;
			}

			DirectoryInfo[] Folders = Dir.GetDirectories();
			foreach( DirectoryInfo di in Folders )
			{
				runningTotal = GetDirectorySize( di, runningTotal );
			}

			return runningTotal;
		}

		#region Spare Code

		//[Obsolete]
		//public static FILETIME DateTimeToFileTime( DateTime fileDate )
		//{
		//	FILETIME ft = new FILETIME();
		//	Int64 fileDateLong = fileDate.ToFileTimeUtc();
		//	ft.dwLowDateTime = ( Int32 )( fileDateLong & 0xFFFFFFFF );
		//	ft.dwHighDateTime = ( Int32 )( fileDateLong >> 32 );

		//	return ft;
		//}

		//[Obsolete]
		//public static Int64 FileTimeToLong( FILETIME ft )
		//{
		//	return ( ( ( Int64 )ft.dwHighDateTime ) << 32 ) + ( ft.dwLowDateTime );
		//}

		//[Obsolete]
		//public static string GetFullPath( string path )
		//{
		//	// Validate
		//	if( string.IsNullOrEmpty( path ) ) return null;

		// UInt32 buffSize = ( UInt32 )MAX_PATH; UInt32 neededSize = buffSize;

		// StringBuilder buff = null;

		// // Save the name part - everything after the last backslash, if any. // This is needed
		// because if the name ends in trailing whitespace, the // GetFullPathName Win32 API will
		// not work properly, as it strips the whitespace. // // So, we save the file name, take the
		// rest of the path and convert it to a full path, // then append the file name and return
		// the result. // string NamePart = String.Empty; Int32 loc = path.LastIndexOf( "\\",
		// StringComparison.OrdinalIgnoreCase ); if( loc > 0 ) { NamePart = path.Substring( loc + 1
		// ); path = path.Substring( 0, loc ); }

		// // Special handling for root paths path = Regex.Replace( path, @"^\\\\\?\\UNC\\", @"\\"
		// ); path = Regex.Replace( path, @"^\\\\\?\\", String.Empty );

		// if( Regex.IsMatch( path, @"^.:?$" ) ) { path += "\\"; }

		// buff = new StringBuilder( ( Int32 )buffSize ); neededSize =
		// NativeMethods.GetFullPathName( path, buffSize, buff, IntPtr.Zero ); if( neededSize == 0 )
		// { throw new Win32Exception( Marshal.GetLastWin32Error() ); } else { if( buffSize <
		// neededSize ) { // Increase the buffer to the necessary size and try again buffSize =
		// neededSize; buff = new StringBuilder( ( Int32 )buffSize ); neededSize =
		// NativeMethods.GetFullPathName( path, buffSize, buff, IntPtr.Zero );

		// if( neededSize == 0 ) { throw new Win32Exception( Marshal.GetLastWin32Error() ); }

		// Debug.Assert( neededSize <= buffSize ); } }

		// if( buff != null ) { if( !String.IsNullOrEmpty( NamePart ) ) { // Add the name part back
		// unchanged if( buff[ buff.Length - 1 ] != '\\' ) { buff.Append( "\\" + NamePart ); } else
		// { buff.Append( NamePart ); } }

		// return buff.ToString(); }

		//	return null;
		//}

		///// <summary>
		///// The ToShortPathName function retrieves the Int16 path from of a specified Int64 input path
		///// </summary>
		///// <param name="longName">The Int64 name path</param>
		///// <returns>A Int16 name path string</returns>
		///// <remarks>The GetShortPathName API requires the target to exist</remarks>

		///// <summary>
		///// Determines the key that a given PE file was xor'd with
		///// </summary>
		///// <param name="strm"></param>
		///// <param name="PEOnly">If false, then matching MZ at the start of the file will be considered sufficient - this may be prone to false positives. If the PEOnly flag is true, then we will ensure that it's a PE file - this is less error prone.</param>
		///// <param name="closeReader"></param>
		///// <returns></returns>
		//[Obsolete]
		//public static Byte GetXorKeyFromStream( Stream strm, Boolean PEOnly, Boolean closeReader )
		//{
		//	const Int32 PE_Header_Offset = 0x3C;
		//	const UInt16 MZ_Magic = 0x5A4D;
		//	const Int32 PE_Magic = 0x4550;

		// if( strm == null ) { throw new ArgumentNullException( nameof( strm ) ); }

		// Byte[] tempbuff = new Byte[] { 0, 0, 0, 0 };

		// if( ( strm.Length < 4 ) ) { return 0; }

		// BinaryReader reader = new BinaryReader( strm );

		// try { strm.Seek( 0, SeekOrigin.Begin );

		// // Read the first two bytes and compute the candidate key UInt32 val =
		// reader.ReadUInt16(); Byte key = ( Byte )( val ^ MZ_Magic );

		// // Get a 2-Byte and 4-Byte version of the key so we can xor // more than one Byte at a
		// time tempbuff[ 0 ] = key; tempbuff[ 1 ] = key; tempbuff[ 2 ] = 0; tempbuff[ 3 ] = 0;

		// UInt16 ShortKey = BitConverter.ToUInt16( tempbuff, 0 );

		// tempbuff[ 2 ] = key; tempbuff[ 3 ] = key;

		// UInt32 IntKey = BitConverter.ToUInt32( tempbuff, 0 );

		// // If the value is not 'MZ', we don't have a match if( ( val ^ ShortKey ) != MZ_Magic ) {
		// return 0; }

		// if( !PEOnly ) { return key; }

		// // Make sure we won't read past the end of the file if( ( PE_Header_Offset ) > (
		// strm.Length - 4 ) ) { return 0; }

		// // Seek to the specified offset strm.Seek( PE_Header_Offset, SeekOrigin.Begin );

		// // Read four bytes and unscramble them to get the offset of the PE header UInt32 PEOffset
		// = ( ( reader.ReadUInt32() ) ^ IntKey );

		// // Make sure we won't read past the end of the file if( PEOffset > strm.Length - 4 ) {
		// return 0; }

		// // Seek to the PE header strm.Seek( PEOffset, SeekOrigin.Begin );

		// // Read four bytes and unscramble them val = ( reader.ReadUInt32() ) ^ IntKey;

		// // If the value is 'PE00' then we have a match if( val == PE_Magic ) { return key; } }
		// finally { if( closeReader ) { reader.Close(); } }

		//	return 0;
		//}

		///// <summary>
		///// Adds the \\?\ prefix if necessary and performs other actions to put the path
		///// in a canonical form before passing it to the CreateFile and other Win32 APIs.
		///// </summary>
		///// <param name="path"></param>
		///// <returns></returns>
		//[Obsolete]
		//public static string NormalizePathForWin32( string path )
		//{
		//	string newpath = GetFullPath( path );

		// if( newpath.Contains( "/" ) ) { newpath = newpath.Replace( "/", @"\" ); }

		// if( !newpath.StartsWith( @"\\?\" ) ) { if( newpath.StartsWith( @"\\" ) ) { // UNC Path
		// newpath = String.Format( @"{0}{1}", @"\\?\UNC\", newpath.Substring( 2 ) ); } else {
		// newpath = String.Format( @"{0}{1}", @"\\?\", newpath ); } }

		//	return newpath;
		//}

		///// <summary>
		///// Check for the presence of any 'bad' characters in the string specified
		///// </summary>
		//[Obsolete]
		//public static Boolean HasBadChars( string s )
		//{
		//	char[] chars = s.ToCharArray();
		//	for( Int32 i = 0; i < chars.Length; i++ )
		//	{
		//		Int32 val = ( Int32 )chars[ i ];
		//		if( val <= 32 )
		//		{
		//			return true;
		//		}

		// foreach( char c in badchars ) { if( ( Int32 )c == val ) { return true; } } }

		//	return false;
		//}

		//[Obsolete]
		//public static string ToShortPathName( string longName )
		//{
		//	String longNameBuffer;

		// if( !longName.StartsWith( @"\\?\" ) ) { longNameBuffer = @"\\?\" + longName;

		// // Handle UNC paths if( longNameBuffer.StartsWith( @"\\?\\\" ) ) { longNameBuffer =
		// longNameBuffer.Replace( @"\\?\\\", @"\\?\UNC\" ); } } else { longNameBuffer = longName; }

		// StringBuilder shortNameBuffer = new StringBuilder( 256 ); Int32 bufferSize = shortNameBuffer.Capacity;

		// Int32 result = NativeMethods.GetShortPathName( longNameBuffer, shortNameBuffer,
		// bufferSize ); Int32 err = Marshal.GetLastWin32Error();

		// if( result == 0 ) { // Failed to convert to a Int16 name. Does the file exist? throw new
		// Exception( "P/Invoke call to GetShortPathName() failed with error " + err + "." ); }

		// string shortName = shortNameBuffer.ToString();

		// if( !longName.StartsWith( @"\\?\" ) && shortName.StartsWith( @"\\?\" ) ) { // The
		// original path didn't have the \\?\ prefix, so remove it

		// if( shortName.StartsWith( @"\\?\UNC\" ) ) { shortName = shortName.Replace( @"\\?\UNC\",
		// @"\\" ); } else { shortName = shortName.Replace( @"\\?\", String.Empty ); } }

		//	return shortName;
		//}

		//// Recursively renames any files or folders under the specified path
		//// that cannot be handled properly by .NET
		////
		//[Obsolete]
		//public static void RenameBadFilePaths( string pathname )
		//{
		//	DirectoryInfoEx di = new DirectoryInfoEx( pathname );
		//	if( !di.Exists )
		//	{
		//		return;
		//	}

		// Regex EndsWithSpace = new Regex( @"^.+\s+$" );

		// // First rename the root directory if necessary if( HasBadChars( di.FullName ) ||
		// EndsWithSpace.IsMatch( di.FullName ) ) { string newrootname = GetBetterFileName(
		// di.FullName ); MoveItem( di.FullName, newrootname ); di = new DirectoryInfoEx(
		// newrootname );

		// Debug.Assert( !( HasBadChars( di.FullName ) ) && !( EndsWithSpace.IsMatch( di.FullName )
		// ) ); }

		// FileInfoEx[] childrenfiles = di.GetFiles(); DirectoryInfoEx[] childrendirs = di.GetDirectories();

		// List<object> children = new List<object>(); children.AddRange( childrenfiles );
		// children.AddRange( childrendirs );

		// foreach( object child in children ) { string childpath = String.Empty;

		// if( ( child as FileInfoEx ) != null ) { childpath = ( ( FileInfoEx )child ).FullName; }

		// if( ( child as DirectoryInfoEx ) != null ) { childpath = ( ( DirectoryInfoEx )child
		// ).FullName; }

		// if( !HasBadChars( childpath ) && !EndsWithSpace.IsMatch( childpath ) ) { continue; }

		// string bettername = GetBetterFileName( childpath );

		// MoveItem( childpath, bettername ); }

		//	foreach( DirectoryInfoEx subfolder in childrendirs )
		//	{
		//		RenameBadFilePaths( subfolder.FullName );
		//	}
		//}

		//[Obsolete]
		//public static void MoveItem( string oldpath, string newpath )
		//{
		//	string win32oldpath = NormalizePathForWin32( oldpath );
		//	string win32newpath = NormalizePathForWin32( newpath );
		//	Boolean result = NativeMethods.MoveFile( win32oldpath, win32newpath );

		//	if( !result )
		//	{
		//		string errmsg = String.Format( "MoveFile failed with error {0} trying to move '{1}' to '{2}'", Marshal.GetLastWin32Error(), win32oldpath, win32newpath );
		//		throw new Exception( errmsg );
		//	}
		//}

		//[Obsolete]
		//public static string GetBetterFileName( string name )
		//{
		//	char[] chars = name.ToCharArray();
		//	char replacechar = '_';

		// for( Int32 i = 0; i < chars.Length; i++ ) { char c = chars[ i ]; Int32 val = ( Int32 )c;
		// if( val <= 32 ) { chars[ i ] = replacechar; continue; }

		// foreach( char bc in badchars ) { if( c == bc ) { chars[ i ] = replacechar; break; } } }

		// string bettername = new string( chars ); bettername = bettername.Trim();

		// string tempname = bettername; Int32 counter = 0;

		// Boolean exists = true; while( exists ) { FileInfoEx checkfile = new FileInfoEx( tempname
		// ); DirectoryInfoEx checkdir = new DirectoryInfoEx( tempname );

		// if( !checkfile.Exists && !checkdir.Exists ) { exists = false; continue; }

		// tempname = bettername + counter++; }

		// bettername = tempname;

		// return bettername;

		//}

		///// <summary>
		///// Computes both the MD5 and SHA1 hashes for a file
		///// </summary>
		///// <param name="szPath"></param>
		///// <returns></returns>
		//[Obsolete]
		//public static string[] CalculateHashes( string szPath )
		//{
		//	FileInfoEx fi = new FileInfoEx( szPath );

		//	Stream fs = fi.Open( FileMode.Open, System.IO.FileAccess.Read );
		//	using( fs )
		//	{
		//		string[] result = CalculateHashes( fs );
		//		return result;
		//	}
		//}

		//[Obsolete]
		//public static string[] CalculateHashes( Stream stream )
		//{
		//	string szSha1;
		//	string szMD5;

		// Byte[] bytes;

		// if( stream == null ) { throw new ArgumentNullException( nameof( stream ) ); }

		// stream.Seek( 0, SeekOrigin.Begin ); SHA1CryptoServiceProvider sha1 = new
		// SHA1CryptoServiceProvider(); using( sha1 ) { bytes = sha1.ComputeHash( stream ); }

		// szSha1 = BitConverter.ToString( bytes ); szSha1 = szSha1.Replace( "-", string.Empty ).ToLower();

		// stream.Seek( 0, SeekOrigin.Begin ); MD5CryptoServiceProvider md5 = new
		// MD5CryptoServiceProvider(); using( md5 ) { bytes = md5.ComputeHash( stream ); }

		// szMD5 = BitConverter.ToString( bytes ); szMD5 = szMD5.Replace( "-", string.Empty ).ToLower();

		// string[] result = { szSha1, szMD5 }; return result;

		//}

		///// <summary>
		///// The following is a small wrapper around the regular CalculateHashes function.
		///// It has some retry logic for code that may call it while the target
		///// file may be held in use at periodic intervals.
		///// </summary>
		///// <param name="szPath"></param>
		///// <param name="MaxRetries"></param>
		///// <returns></returns>
		//[Obsolete]
		//public static string[] CalculateHashesWithRetry( string szPath, Int32 MaxRetries )
		//{
		//	string[] result = null;
		//	Int32 RetryCount = 0;
		//	while( RetryCount <= MaxRetries )
		//	{
		//		try
		//		{
		//			result = CalculateHashes( szPath );
		//			break;
		//		}
		//		catch( Exception )
		//		{
		//			if( RetryCount < MaxRetries )
		//			{
		//				System.Threading.Thread.Sleep( SHORT_PAUSE );
		//				RetryCount++;
		//				continue;
		//			}
		//			else
		//			{
		//				throw;
		//			}
		//		}
		//	}

		//	return result;
		//}

		///// <summary>
		///// Calculate hashes with retry logic, using a default number of maximum times to retry
		///// </summary>
		///// <param name="szPath"></param>
		///// <returns></returns>
		//[Obsolete]
		//public static string[] CalculateHashesWithRetry( string szPath )
		//{
		//	const Int32 DEFAULT_MAX_RETRIES = 10;
		//	return CalculateHashesWithRetry( szPath, DEFAULT_MAX_RETRIES );
		//}

		//// Helper function to retrieve all files recursively from
		//// a folder.
		//[Obsolete]
		//public static List<FileInfoEx> GetAllFiles( string FolderPath )
		//{
		//	List<FileInfoEx> FileList = new List<FileInfoEx>();
		//	DirectoryInfoEx di = new DirectoryInfoEx( FolderPath );
		//	FileList.AddRange( di.GetFiles() );

		// foreach( DirectoryInfoEx di2 in di.GetDirectories() ) { string subdirpath = di2.FullName;

		// // The following check is due to a bug in .NET - if the FolderPath name contains // the
		// character 0xA0 (non-breaking space), the call to GetDirectories will // incorrectly
		// return an entry even when no subdirectories are present, and // the subdirectory name
		// returned will also contains 0xA0, which could cause us // to recurse infinitely until we
		// stack overflow. See SBTU bug# 39320. // if( HasBadChars( subdirpath ) ) { subdirpath =
		// ToShortPathName( subdirpath ); }

		// if( !DirectoryEx.Exists( subdirpath ) ) { continue; }

		// FileList.AddRange( GetAllFiles( subdirpath ) ); }

		//	return FileList;
		//}

		#endregion
	}
}