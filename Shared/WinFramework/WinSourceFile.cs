namespace Microsoft.OSGSA.Shared.Framework.FileSystemUtilities
{
	// IF THIS IS NEEDED, MOVE TO THE APP-SPECIFIC FRAMEWORK DIR

	/// <summary>
	/// Represents a text file involved in building the Windows OS: raw, compiled, generated, etc.
	/// </summary>
	///
	//public sealed class WinSourceFile
	//{
	//	#region Fields and Constructors
	//	/// <summary>
	//	/// List of unique file extensions appearing in the PDBs
	//	/// </summary>
	//	public static readonly string[] PDB_FILE_EXTENSIONS = new string[]
	//	{
	//		"AMD64","ASM","BUFHDR","C","CNV","CPP","CPP_UNPROCESSED","CS","CSPP","CXX","D32","DAT","DBG","DEF",
	//		"DLL","DOC","H","HDL","HH","HH2","HI","HM","HPP","HU","HXX", "I","IMP", "INC", "INL", "KEY",
	//		"METADATA_DLL", "MSG", "NETMODULE",	"NT", "NTDEF", "PCH", "RC", "RCH", "RCV", "RES", "RH",
	//		"S","SIMPLE", "SRC","TBL", "TLB", "TLH", "TLI","TMH","TMP","TXT","VB","VER","W","X",
	//		"XAML","XMLBACKUP", "Y"
	//	};
	//	private string rawSourcePath;
	//	private SourceFileSource howfound;
	//	private string canonicalPath;
	//	public string fileName;
	//	public string extension;
	//	private RootFolderClass m_RootFolder = RootFolderClass.Unknown;

	// public WinSourceFile( string rawSourcePath, SourceFileSource HowFound ) { if(
	// string.IsNullOrEmpty( rawSourcePath ) ) { throw new ArgumentNullException( nameof( path ) ); }

	// // Get rid of white space around the path, and any random tabs // char tab = '\u0009'; //
	// this.rawSourcePath = rawSourcePath.Trim().Replace( tab, string.Empty ); this.rawSourcePath =
	// ScrubSourcePath( rawSourcePath ); this.howfound = HowFound; } #endregion

	// public SourceFileSource HowFound { get { return this.howfound; } }

	// public string RawFilePath { get { return rawSourcePath; } }

	// public string FileName { get { if( fileName == null ) { try { fileName = Path.GetFileName(
	// rawSourcePath ); } catch { fileName = "?"; CommonEx.DrawWarning( "Cannot obtain path from " +
	// rawSourcePath ); } } return fileName; } }

	// public string Extension { get { if( extension == null ) { try { extension =
	// Common.GetExtension( rawSourcePath ); } catch { extension = "?"; CommonEx.DrawWarning(
	// "Cannot obtain extension from " + rawSourcePath ); } } return extension; } }

	// // IF THIS IS NEEDED, MOVE TO THE APP-SPECIFIC FRAMEWORK DIR //public string CanonicalPath
	// //{ // /* This can be all sorts of weird things, since we get the path from raw source, pdbs,
	// tool runs, etc. */

	// // get // { // if( canonicalPath == null ) // { // // Lower case, correct slashes //
	// canonicalPath = rawSourcePath.ToLower().Replace( @"/", @"\" );

	// // // Handle the root path based on the source of this file // switch( howfound ) // { //
	// case SourceFileSource.Keph: // case SourceFileSource.Prefast:

	// // // Compile paths will be in one of: // // SourceRoot, ObjRoot or PubRoot // //List<string>
	// rootFolders = new List<string>(); // //rootFolders.Add(
	// FileSystem.WindowsSourceRootFolder.FullName ); // //rootFolders.Add(
	// FileSystem.BuildObjFolder.FullName ); // //rootFolders.Add(
	// FileSystem.BuildPublicFolder.FullName ); // //canonicalPath = KephCommon.SourceCanonicalizer(
	// canonicalPath, rootFolders );

	// // if( canonicalPath.StartsWith( Config.FileSystem.SourceRootDirectory.FullName ) ) // { //
	// canonicalPath = canonicalPath.Replace( Config.FileSystem.SourceRootDirectory.FullName,
	// string.Empty ); // } // else if( canonicalPath.StartsWith(
	// Config.FileSystem.BuildObjFolder.FullName ) ) // { // canonicalPath = canonicalPath.Replace(
	// Config.FileSystem.BuildObjFolder.FullName, string.Empty ); // } // else if(
	// canonicalPath.StartsWith( Config.FileSystem.BuildPublicFolder.FullName ) ) // { //
	// canonicalPath = canonicalPath.Replace( Config.FileSystem.BuildPublicFolder.FullName,
	// string.Empty ); // } // else canonicalPath = null;

	// // break;

	// // case SourceFileSource.RawSourceFile: // Debug.Fail( "Not used for Raw source files" ); // break;

	// // case SourceFileSource.Pdb: // Debug.Fail( "Not used for Pdbs" ); // break;

	// // case SourceFileSource.LocalTool: // Debug.Fail( "Not used for Local Tool source files" );
	// // break; // }

	// // // Ensure no leading slash // if( canonicalPath.StartsWith( @"\" ) ) // { // canonicalPath
	// = canonicalPath.Remove( 0, 1 ); // } // }

	// // return canonicalPath; // } //}

	// ///
	// <summary>
	// /// Hash of canonicalized source path if it exists, of raw source path if it doesn't ///
	// </summary>
	// public Int64 CanonicalFileID { get { Int64 retVal = 0;

	// if( this.CanonicalPath == null ) { // IANHELLE_TODO: don't want Framework to reference
	// anything other than CLR if at all possible, // so I've replaced this with a call to the local
	// hasher // return ROM.SourceFile.GetCanonicalIdentifier( null, this.RawFilePath ); retVal =
	// Hash.HashString64( this.RawFilePath ); } else { // IANHELLE_TODO: don't want Framework to
	// reference anything other than CLR if at all possible // so I've replaced this with a call to
	// the local hasher // return ROM.SourceFile.GetCanonicalIdentifier( null, this.CanonicalPath );
	// retVal = Hash.HashString64( this.CanonicalPath ); }

	// Debug.Assert( retVal != 0 ); return retVal; } }

	// ///
	// <summary>
	// /// Root folder class in which Source file is located /// (Enumerated type
	// SourceFile.RootFolderClass) ///
	// </summary>
	// private RootFolderClass RootFolderType { get { return m_RootFolder; } }

	// ///
	// <summary>
	// /// Root folder path in which Source file is located; ///
	// </summary>
	// public string RootFolder { get { switch( m_RootFolder ) { case RootFolderClass.Source: return
	// Config.FileSystem.SourceRootDirectory.FullName; case RootFolderClass.Object: return
	// Config.FileSystem.BuildObjFolder.FullName; case RootFolderClass.Public: return
	// Config.FileSystem.BuildPublicFolder.FullName; case RootFolderClass.Other: throw new
	// NotImplementedException(); // return HARD_CODED_OTHER_PATH;
	// default: return null; } } private set { ; } }

	// #region Public Statics and Enums

	// public enum SourceFileSource { None, RawSourceFile, Pdb, Keph, LocalTool, Prefast }

	// public enum RootFolderClass { Unknown, Source, Object, Public, Other }

	// public enum MatchMethod { Hash = 0, CanPath = 1, PdbPath = 2, }

	// public enum SourceFileHowFound { PdbScan = 0, FunctionScan = 1, ToolRun = 2, }

	// ///
	// <summary>
	// /// Removes source root, initial slash if it exists, lower cases, and replaces all whacks
	// with forward slashes ///
	// </summary>
	// ///
	// <param name="rawSourcePath">E.g., d:\syncroot\source\ds\dns\server\foo.c</param>
	// ///
	// <returns>E.g., ds\dns\server\foo.c</returns>
	// public static string ScrubSourcePath( string rawSourcePath, Boolean localFilePath = false ) {
	// if( rawSourcePath == null ) return null;

	// string ret = rawSourcePath.ToLower() .Replace( "/", @"\" ) .Trim();

	// if( localFilePath ) { ret = ret.Replace(
	// Config.FileSystem.SourceRootDirectory.FullName.ToLower(), string.Empty ); }

	// if( ret.StartsWith( @"\" ) ) { ret = ret.Substring( 1, ret.Length - 1 ); }

	// return ret; } private static Regex regexSymbolFullPath = new Regex(
	// @"^(.+)\\([a-zA-Z0-9_]*)\\([^\\]+)\.pdb$", RegexOptions.Compiled | RegexOptions.IgnoreCase );
	// public static string ConvertSymbolFullPathToBinaryFullPath( string symbolPath ) { // Convert:
	// D:\Win7\Symbols\Amd64FreClient_en_us\ReleaseSymbols\Windows\System32\dll\ntdll.pdb // To: D:\Win7\Media\Amd64FreClient_en_us\Release\Windows\System32\ntdll.dll

	// string ret = symbolPath.ToLower().Replace( "releasesymbols", "release" ).Replace( "symbols",
	// "media" ); Match match = regexSymbolFullPath.Match( ret );

	// ret = string.Format( "{0}\\{1}.{2}", match.Groups[ 1 ].Value, match.Groups[ 3 ].Value,
	// match.Groups[ 2 ].Value );

	//		return ret;
	//	}
	//	#endregion
	//}
}