using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	/// <summary>
	/// Used to abstract the difference between filesystem that could happen on the same machine or
	/// different machines
	/// </summary>
	public abstract class VirtualPath
	{
		#region Fields and Constructors

		protected readonly string hostName = null;
		protected readonly string localPath = null;
		protected readonly string remotePath = null;
		protected readonly DriveLetter driveLetter = DriveLetter.D;
		protected readonly Boolean isFile = false;

		/// <summary>
		/// Abstracts the location of a filesystem object path, whether it's local or remote or a
		/// file or a directory, and the approprite local or remote UNC is returned
		/// </summary>
		/// <param name="hostMachine">The machine hosting the file path</param>
		/// <param name="localPath">
		/// Either a local path (e.g., 'c:\foo\bar\this.txt' or a single drive letter, e.g., 'D'
		/// </param>
		/// <param name="verifyExistence">
		/// If TRUE, validates that the virtual .Path exists, throws an exception if not
		/// </param>
		public VirtualPath
		(
			string hostMachine,
			string localPath,
			Boolean isFile,
			Boolean throwOnNotExists = false )
		{
			if( string.IsNullOrEmpty( hostMachine ) )
			{
				throw new ArgumentNullException( nameof( hostMachine ) );
			}
			else if( string.IsNullOrEmpty( localPath ) )
			{
				throw new ArgumentNullException( nameof( localPath ) );
			}
			else if( !Utilities.IsLocalPath( localPath ) )
			{
				throw new ArgumentException( "Must be local path", nameof( localPath ) );
			}

			this.hostName = hostMachine.ToUpper();
			this.isFile = isFile;

			// So Int64 as the path is a UNC path on the same machine as hostMachine, we can deal
			if( CommonRegex.UncPathWithDriveDollarRegex.IsMatch( localPath ) )
			{
				Match m = CommonRegex.UncPathWithDriveDollarRegex.Match( localPath );
				Debug.Assert( m.Groups[ 1 ].Value == this.hostName );

				this.localPath = m.Groups[ 2 ].Value.Replace( "$", ":" );
			}
			else
			{
				this.localPath = localPath;
			}

			if( this.LocalPath.Length == 1 )
			{
				// 'C' on 'MACHINE' -> \\MACHINE\c$
				this.remotePath = string.Format( @"\\{0}\{1}$", this.hostName, this.LocalPath );

				// 'C' -> C:
				this.localPath += ":"; // TODO or should this be C:\

				this.driveLetter = ( DriveLetter )Enum.Parse
				(
					typeof( DriveLetter ),
					this.LocalPath.Substring( 0, 1 ),
					true
				);
			}
			else
			{
				// 'C:' on 'MACHINE' -> \\MACHINE\C$
				// c: \foo\bar.txt on MACHINE -> \\MACHINE\c$\foo\bar.txt
				this.remotePath = string.Format( @"\\{0}\{1}", this.HostName, this.LocalPath.Replace( ":", "$" ) );
				this.localPath = localPath.Replace( "$", ":" );
				this.driveLetter = ( DriveLetter )Enum.Parse
				(
					typeof( DriveLetter ),
					this.RemotePath[ this.RemotePath.IndexOf( '$' ) - 1 ].ToString(),
					true // IgnoreCase
				);
			}

			if( throwOnNotExists && !this.Exists )
			{
				throw new ScarabException( "Path '{0}' not found", this.Path );
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Calculates a path using local or remote syntax depending on caller relative to the server
		/// </summary>
		public string Path
		{
			get
			{
				// Allow for the SQL Server localhost moniker
				Boolean isLocal = ( 0 == string.Compare( this.HostName, Environment.MachineName, true )
										|| 0 == string.Compare( this.HostName, "(local)", true ) );

				if( isLocal )
				{
					Debug.Assert( this.LocalPath[ 1 ] == ':' );
					return this.LocalPath;
				}
				else
				{
					return this.RemotePath;
				}
			}
		}

		public string LocalPath
		{
			get { return this.localPath; }
		}

		public string RemotePath
		{
			get { return this.remotePath; }
		}

		public string RelativePath
		{
			get { return this.localPath.Substring( 2 ); }
		}

		/// <summary>
		/// DNS or NetBIOS name of the machine hosting the directory
		/// </summary>
		public string HostName
		{
			get { return this.hostName; }
		}

		public DriveLetter DriveLetter
		{
			get { return this.driveLetter; }
		}

		/// <summary>
		/// Returns TRUE if this VirtualPath is to a file, FALSE if it's a directory
		/// </summary>
		public Boolean IsFile
		{
			get { return this.isFile; }
		}

		/// <summary>
		/// Whether the directory or file exists
		/// </summary>
		public Boolean Exists
		{
			get
			{
				if( this.IsFile )
				{
					return File.Exists( this.Path );
				}
				else
				{
					return Directory.Exists( this.Path );
				}
			}
		}

		#endregion

		#region Public Methods

		///// <summary>
		///// Can only be used for directories, creates the dir if it does not exist
		///// </summary>
		//public void Ensure()
		//{
		//	if( !this.IsFile && !this.Exists )
		//	{
		//		Directory.CreateDirectory( this.Path );
		//	}
		//}

		/// <summary>
		/// Appends a path to the current existing virtual path that's a directory (similar usage to
		/// Path.Combine wrt adding a directory onto the end of a file path). Appending a path to a
		/// file with result in an exception.
		/// </summary>
		/// <param name="pathOrFileName">The path to append; can be file or directory</param>
		/// <param name="validate">
		/// If TRUE, validates that the virtual .Path exists, throws an exception if not
		/// </param>
		/// <returns>A new VirtualPath object</returns>
		//public VirtualPath Append( string pathOrFileName, Boolean validate = false )
		//{
		//	Debug.Assert( !string.IsNullOrWhiteSpace( pathOrFileName ) );

		// if( this.IsFile ) { throw new ScarabException( "Cannot append a file or a path to a file"
		// ); }

		// Boolean incomingIsFile = CommonRegex.ValidFilePathRegex.IsMatch( pathOrFileName );
		// VirtualPath ret = null;

		// string originalPath = this.LocalPath.EndsWith( ":" ) // E.g., Make F:\ out of F: ?
		// this.LocalPath + @"\" : this.LocalPath;

		// if( incomingIsFile ) { string newPath = System.IO.Path.Combine( originalPath,
		// pathOrFileName ); ret = new VirtualFilePath( this.HostName, newPath, throwOnNotExists:
		// validate ); } else { if( pathOrFileName.StartsWith( @"\" ) ) // Remove a preceding slash,
		// if one exists { pathOrFileName = pathOrFileName.Substring( 1 ); }

		// string newPath = System.IO.Path.Combine( originalPath, pathOrFileName );

		// ret = new VirtualDirectoryPath( this.HostName, newPath, throwOnNotExists: validate ); }

		//	return ret;
		//}

		//public void SetReadOnlyAttribute( Boolean readOnly )
		//{
		//	FileAttributes attributes = File.GetAttributes( this.Path );

		//	if( !readOnly && ( attributes & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly )
		//	{
		//		attributes = RemoveAttribute( attributes, FileAttributes.ReadOnly );
		//		File.SetAttributes( this.Path, attributes );
		//		fr.Common.WriteVerboseLine( "The file '{0}' is no longer read-only", this.Path );
		//	}
		//	else if( readOnly && ( attributes & FileAttributes.ReadOnly ) != FileAttributes.ReadOnly )
		//	{
		//		// Hide the file.
		//		File.SetAttributes( this.Path, File.GetAttributes( this.Path ) | FileAttributes.ReadOnly );
		//		fr.Common.WriteVerboseLine( "The file '{0}' is now read-only", this.Path );
		//	}
		//}

		#endregion

		#region Converters

		//public sealed class VirtualPathConverter : ConfigurationConverterBase
		//{
		//	public Boolean ValidateType( object value, Type expected )
		//	{
		//		return !( value != null && value.GetType() != expected );
		//	}

		// ///
		// <summary>
		// /// This method overrides CanConvertTo from TypeConverter. This is called when someone
		// /// wants to convert an instance of VirtualPath to another type. Here, only ///
		// conversion to an InstanceDescriptor is supported. ///
		// </summary>
		// ///
		// <param name="context"></param>
		// ///
		// <param name="destinationType"></param>
		// ///
		// <returns></returns>
		// public override Boolean CanConvertTo( ITypeDescriptorContext context, Type
		// destinationType ) { if( destinationType == typeof( InstanceDescriptor ) ) { return true; }

		// // Always call the base to see if it can perform the conversion. return
		// base.CanConvertTo( context, destinationType ); }

		// public override Boolean CanConvertFrom( ITypeDescriptorContext ctx, Type type ) { if(
		// type == typeof( string ) ) { return true; } else { return base.CanConvertFrom( ctx, type
		// ); } }

		// ///
		// <summary>
		// /// This code performs the actual conversion from a VirtualPath to an InstanceDescriptor. ///
		// </summary>
		// public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture,
		// object value, Type destinationType ) { if( destinationType == typeof( InstanceDescriptor
		// ) ) { ConstructorInfo ci = typeof( VirtualPath ).GetConstructor ( new Type[] { typeof(
		// string ), typeof( string ), typeof( Boolean ) } );

		// VirtualPath t = ( VirtualPath )value;

		// return new InstanceDescriptor( ci, new object[] { t.HostName, t.LocalPath, false } ); }

		// // Always call base, even if you can't convert. return base.ConvertTo( context, culture,
		// value, destinationType ); }

		// //public override object ConvertFrom //( // ITypeDescriptorContext ctx, // CultureInfo
		// ci, // object data ) //{ // VirtualPath vfp = VirtualPath.Parse // ( // data.ToString(),
		// // true, // Just 'throwOnInvalidPath' can be true, since file paths or // false // ); //
		// Debug.Assert( vfp != null );

		//	//	return vfp;
		//	//}
		//}

		#endregion

		#region Overrides

		public override string ToString()
		{
			return this.Path;
		}

		public override Int32 GetHashCode()
		{
			return Hash.HashString32( this.Path );
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false
			if( obj == null )
			{
				return false;
			}

			VirtualPath other = obj as VirtualPath;

			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return this.GetHashCode() == other.GetHashCode();
		}

		#endregion

		#region Statics

		/// <summary>
		/// Converts a string to a VirtualFilepath instance
		/// </summary>
		/// <param name="path">
		/// Can be a local path (in which case the local machine is the host) or "MACHINENAME,c:\local\path"
		/// </param>
		/// <param name="throwOnInvalidPath">
		/// throw exception if the path is not syntactically valid
		/// </param>
		/// <param name="throwIfNotDirectoryPath">throw exception is path is to a file</param>
		/// <param name="throwOnNotExists">throw exception if the directory or file does not exist</param>
		/// <returns>A VirtualPath object</returns>
		//public static VirtualPath Parse
		//(
		//	string path,
		//	Boolean throwOnInvalidPath = false,
		//	Boolean throwOnNotExists = false )
		//{
		//	string hostName = System.Environment.MachineName;
		//	string localPath = null;

		// #region Validate

		// if( string.IsNullOrWhiteSpace( path ) ) { throw new ArgumentNullException( nameof( path )
		// ); } else if( path.StartsWith( @"\\" ) ) { throw new fr.ScarabException( "UNC paths are
		// not supported" ); }

		// if( path.Contains( "," ) ) { var split = path.Split( new string[] { "," },
		// StringSplitOptions.RemoveEmptyEntries ); hostName = split[ 0 ]; localPath =
		// System.IO.Path.GetFullPath( split[ 1 ] ); } else { localPath =
		// System.IO.Path.GetFullPath( path ); }

		// if( !CommonRegex.LocalFsoPathRegex.IsMatch( localPath ) ) { if( throwOnInvalidPath ) {
		// throw new ScarabException( "Failed to parse path '{0}'", path ); } else { return null; } }

		// #endregion

		// VirtualPath ret = new VirtualPath ( hostName, localPath,
		// throwOnNotExists: throwOnNotExists );

		// Debug.Assert( ret != null );

		//	return ret;
		//}

		//private static FileAttributes RemoveAttribute
		//(
		//	FileAttributes attributes,
		//	FileAttributes attributesToRemove )
		//{
		//	return attributes & ~attributesToRemove;
		//}

		/// <summary>
		/// Appends a path to an existing virtual path. Similar usage to Path.Combine wrt adding a
		/// directory onto the end of a file path
		/// </summary>
		//public static VirtualPath operator +( VirtualPath vfp, string path )
		//{
		//	return vfp.Append( path );
		//}

		#endregion
	}
}