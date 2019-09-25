using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;

using fr = Tamasi.Shared.Framework;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	/// <summary>
	/// Used to abstract the difference between filesystem that could happen on the same machine or
	/// different machines
	/// </summary>
	[TypeConverter( typeof( VirtualDirectoryPathConverter ) )]
	public sealed class VirtualDirectoryPath : VirtualPath
	{
		#region Fields and Constructors

		/// <summary>
		/// Abstracts the location of a file path; whether a given path is local or remote is
		/// determined at runtime, and the approprite local or remote UNC is returned
		/// </summary>
		/// <param name="hostMachine">The machine hosting the file path</param>
		/// <param name="localPath">
		/// Either a local path (e.g., 'c:\foo\bar\this.txt' or a single drive letter, e.g., 'D'
		/// </param>
		/// <param name="verifyExistence">
		/// If TRUE, validates that the virtual .Path exists, throws an exception if not
		/// </param>
		public VirtualDirectoryPath
		(
			string hostMachine,
			string localDirectoryPath,
			Boolean throwOnNotExists = false,
			Boolean throwIfNotDirectoryPath = false )
			: base( hostMachine, localDirectoryPath, false, throwOnNotExists )
		{
			if( throwIfNotDirectoryPath && this.IsFile )
			{
				throw new ScarabException( "Path '{0}' is not a directory path", this.Path );
			}
		}

		#endregion

		#region Properties

		public DirectoryInfo AsDirectoryInfo
		{
			get
			{
				DirectoryInfo ret = null;

				if( !this.IsFile )
				{
					ret = new DirectoryInfo( this.Path );
				}

				return ret;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Can only be used for directories, creates the dir if it does not exist
		/// </summary>
		public void Ensure()
		{
			if( !this.Exists )
			{
				Directory.CreateDirectory( this.Path );
			}
		}

		/// <summary>
		/// Appends a path to the current existing virtual path that's a directory (similar usage to
		/// Path.Combine wrt adding a directory onto the end of a file path). Appending a path to a
		/// file with result in an exception.
		/// </summary>
		/// <param name="dirNameOrRelPath">The path to append; can be file or directory</param>
		/// <param name="validate">
		/// If TRUE, validates that the virtual .Path exists, throws an exception if not
		/// </param>
		/// <returns>A new VirtualDirectoryPath object</returns>
		public VirtualDirectoryPath Append( string dirNameOrRelPath, Boolean validate = false )
		{
			if( string.IsNullOrWhiteSpace( dirNameOrRelPath ) )
			{
				throw new ArgumentNullException( nameof( dirNameOrRelPath ) );
			}

			if( dirNameOrRelPath.StartsWith( @"\" ) ) // Remove a preceding slash, if one exists
			{
				dirNameOrRelPath = dirNameOrRelPath.Substring( 1 );
			}

			if( !CommonRegex.RelativePathRegex.IsMatch( dirNameOrRelPath ) )
			{
				string message = string.Format
				(
					"'{0}' is not a valid dirname or relative directory path",
					dirNameOrRelPath
				);
				throw new ArgumentException( message, nameof( dirNameOrRelPath ) );
			};

			string originalPath = this.LocalPath.EndsWith( ":" ) // E.g., Make F:\ out of F:
				? this.LocalPath + @"\"
				: this.LocalPath;

			string newPath = System.IO.Path.Combine( originalPath, dirNameOrRelPath );

			return new VirtualDirectoryPath( this.HostName, newPath, throwOnNotExists: validate );
		}

		/// <summary>
		/// Adds a filename or relative filepath to the current VirtualDirectoryPath
		/// and returns a VirtualFilePath
		/// </summary>
		/// <param name="fileNameOrRelPath">a filename (e.g., 'foo.c') or relative filepath ('foo\bar\file.c')</param>
		/// <param name="validate">
		/// If TRUE, validates that the virtual .Path exists, throws an exception if not
		/// </param>
		/// <returns>A new VirtualDirectoryPath object</returns>
		public VirtualFilePath AppendFileNameOrRelPath( string fileNameOrRelPath, Boolean validate = false )
		{
			if( string.IsNullOrWhiteSpace( fileNameOrRelPath ) )
			{
				throw new ArgumentNullException( nameof( fileNameOrRelPath ) );
			}

			if( !CommonRegex.RelativePathRegex.IsMatch( fileNameOrRelPath ) )
			{
				string message = string.Format
				(
					"'{0}' is not a valid filename or relative filepath",
					fileNameOrRelPath
				);
				throw new ArgumentException( message, nameof( fileNameOrRelPath ) );
			};

			string originalPath = this.LocalPath.EndsWith( ":" ) // E.g., Make F:\ out of F:
				? this.LocalPath + @"\"
				: this.LocalPath;

			string newPath = System.IO.Path.Combine( originalPath, fileNameOrRelPath );

			return new VirtualFilePath( this.HostName, newPath, throwOnNotExists: validate );
		}

		#endregion

		#region Converters

		public sealed class VirtualDirectoryPathConverter : ConfigurationConverterBase
		{
			public Boolean ValidateType( object value, Type expected )
			{
				return !( value != null && value.GetType() != expected );
			}

			/// <summary>
			/// This method overrides CanConvertTo from TypeConverter. This is called when someone
			/// wants to convert an instance of VirtualDirectoryPath to another type. Here, only
			/// conversion to an InstanceDescriptor is supported.
			/// </summary>
			/// <param name="context"></param>
			/// <param name="destinationType"></param>
			/// <returns></returns>
			public override Boolean CanConvertTo( ITypeDescriptorContext context, Type destinationType )
			{
				if( destinationType == typeof( InstanceDescriptor ) )
				{
					return true;
				}

				// Always call the base to see if it can perform the conversion.
				return base.CanConvertTo( context, destinationType );
			}

			public override Boolean CanConvertFrom( ITypeDescriptorContext ctx, Type type )
			{
				if( type == typeof( string ) )
				{
					return true;
				}
				else
				{
					return base.CanConvertFrom( ctx, type );
				}
			}

			/// <summary>
			/// This code performs the actual conversion from a VirtualDirectoryPath to an InstanceDescriptor.
			/// </summary>
			public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
			{
				if( destinationType == typeof( InstanceDescriptor ) )
				{
					ConstructorInfo ci = typeof( VirtualDirectoryPath ).GetConstructor
					(
						new Type[]
						{
							typeof( string ),
							typeof( string ),
							typeof( Boolean )
						}
					);

					VirtualDirectoryPath t = ( VirtualDirectoryPath )value;

					return new InstanceDescriptor( ci, new object[] { t.HostName, t.LocalPath, false } );
				}

				// Always call base, even if you can't convert.
				return base.ConvertTo( context, culture, value, destinationType );
			}

			public override object ConvertFrom
			(
				ITypeDescriptorContext ctx,
				CultureInfo ci,
				object data )
			{
				VirtualDirectoryPath vfp = VirtualDirectoryPath.Parse
				(
					data.ToString(),
					true,		// Just 'throwOnInvalidPath' can be true, since file paths or
					false,		// nonexistent paths could legitimately be in config files
					false
				);
				Debug.Assert( vfp != null );

				return vfp;
			}
		}

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

			VirtualDirectoryPath other = obj as VirtualDirectoryPath;

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
		/// <returns>A VirtualDirectoryPath object</returns>
		public static VirtualDirectoryPath Parse
		(
			string path,
			Boolean throwOnInvalidPath = false,
			Boolean throwIfNotDirectoryPath = false,
			Boolean throwOnNotExists = false )
		{
			string hostName = System.Environment.MachineName;
			string localPath = null;

			#region Validate

			if( string.IsNullOrWhiteSpace( path ) )
			{
				throw new ArgumentNullException( nameof( path ) );
			}
			else if( path.StartsWith( @"\\" ) )
			{
				throw new fr.ScarabException( "UNC paths are not supported" );
			}

			if( path.Contains( "," ) )
			{
				var split = path.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries );
				hostName = split[ 0 ];
				localPath = System.IO.Path.GetFullPath( split[ 1 ] );
			}
			else
			{
				localPath = System.IO.Path.GetFullPath( path );
			}

			if( !CommonRegex.LocalFsoPathRegex.IsMatch( localPath ) )
			{
				if( throwOnInvalidPath )
				{
					throw new ScarabException( "Failed to parse path '{0}'", path );
				}
				else
				{
					return null;
				}
			}

			#endregion

			VirtualDirectoryPath ret = new VirtualDirectoryPath
			(
				hostName,
				localPath,
				throwIfNotDirectoryPath: throwIfNotDirectoryPath,
				throwOnNotExists: throwOnNotExists
			);

			Debug.Assert( ret != null );

			return ret;
		}

		private static FileAttributes RemoveAttribute
		(
			FileAttributes attributes,
			FileAttributes attributesToRemove )
		{
			return attributes & ~attributesToRemove;
		}

		/// <summary>
		/// Appends a path to an existing virtual path. Similar usage to Path.Combine wrt adding a
		/// directory onto the end of a file path
		/// </summary>
		public static VirtualPath operator +( VirtualDirectoryPath vfp, string path )
		{
			return vfp.Append( path );
		}

		#endregion

		#region Spare Code

		//if( string.IsNullOrEmpty( hostMachine ) )
		//{
		//	throw new ArgumentNullException( nameof( hostMachine ) );
		//}
		//if( string.IsNullOrEmpty( localPath ) )
		//{
		//	throw new ArgumentNullException( nameof( localPath ) );
		//}

		//this.hostName = hostMachine.ToUpper();

		//// So Int64 as the path is a UNC path on the same machine as hostMachine, we can deal
		//if( CommonRegex.UncPathWithDriveDollarRegex.IsMatch( localPath ) )
		//{
		//	Match m = CommonRegex.UncPathWithDriveDollarRegex.Match( localPath );
		//	Debug.Assert( m.Groups[ 1 ].Value == this.hostName );

		//	this.localPath = m.Groups[ 2 ].Value.Replace( "$", ":" );
		//}
		//else
		//{
		//	this.localPath = localPath;
		//}

		//Debug.Assert( !this.LocalPath.StartsWith( @"\" ) );

		//if( this.LocalPath.Length == 1 )
		//{
		//	// 'C' on 'MACHINE' -> \\MACHINE\c$
		//	this.remotePath = string.Format( @"\\{0}\{1}$", this.hostName, this.LocalPath );

		// // 'C' -> C: this.localPath += ":"; // TODO or should this be C:\

		//	this.driveLetter = ( DriveLetter )Enum.Parse
		//	(
		//		typeof( DriveLetter ),
		//		this.LocalPath.Substring( 0, 1 ),
		//		true
		//	);
		//}
		//else
		//{
		//	// 'C:' on 'MACHINE' -> \\MACHINE\C$
		//	// c: \foo\bar.txt on MACHINE -> \\MACHINE\c$\foo\bar.txt
		//	this.remotePath = string.Format( @"\\{0}\{1}", this.HostName, this.LocalPath.Replace( ":", "$" ) );
		//	this.localPath = localPath.Replace( "$", ":" );
		//	this.driveLetter = ( DriveLetter )Enum.Parse
		//	(
		//		typeof( DriveLetter ),
		//		this.RemotePath[ this.RemotePath.IndexOf( '$' ) - 1 ].ToString(),
		//		true // IgnoreCase
		//	);
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
	}
}