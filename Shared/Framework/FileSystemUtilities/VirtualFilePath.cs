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
	[TypeConverter( typeof( VirtualFilePathConverter ) )]
	public sealed class VirtualFilePath : VirtualPath
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
		public VirtualFilePath
		(
			string hostMachine,
			string localFilePath,
			Boolean throwOnNotExists = false,
			Boolean throwIfNotFilePath = false )
			: base( hostMachine, localFilePath, true, throwOnNotExists )
		{
			if( throwIfNotFilePath && !this.IsFile )
			{
				throw new ScarabException( "Path '{0}' is not a file path", this.Path );
			}
		}

		#endregion

		#region Properties

		public Boolean IsReadOnly
		{
			get
			{
				FileAttributes attributes = File.GetAttributes( this.Path );
				return ( attributes & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly;
			}
		}

		public FileInfo AsFileInfo
		{
			get
			{
				FileInfo ret = null;

				if( this.IsFile )
				{
					ret = new FileInfo( this.Path );
				}

				return ret;
			}
		}

		#endregion

		#region Public Methods

		public void SetReadOnlyAttribute( Boolean readOnly )
		{
			FileAttributes attributes = File.GetAttributes( this.Path );

			if( !readOnly && ( attributes & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly )
			{
				attributes = RemoveAttribute( attributes, FileAttributes.ReadOnly );
				File.SetAttributes( this.Path, attributes );
				fr.Common.WriteVerboseLine( "The file '{0}' is no longer read-only", this.Path );
			}
			else if( readOnly && ( attributes & FileAttributes.ReadOnly ) != FileAttributes.ReadOnly )
			{
				// Hide the file.
				File.SetAttributes( this.Path, File.GetAttributes( this.Path ) | FileAttributes.ReadOnly );
				fr.Common.WriteVerboseLine( "The file '{0}' is now read-only", this.Path );
			}
		}

		#endregion

		#region Converters

		public sealed class VirtualFilePathConverter : ConfigurationConverterBase
		{
			public Boolean ValidateType( object value, Type expected )
			{
				return !( value != null && value.GetType() != expected );
			}

			/// <summary>
			/// This method overrides CanConvertTo from TypeConverter. This is called when someone
			/// wants to convert an instance of VirtualFilePath to another type. Here, only
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
			/// This code performs the actual conversion from a VirtualFilePath to an InstanceDescriptor.
			/// </summary>
			public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
			{
				if( destinationType == typeof( InstanceDescriptor ) )
				{
					ConstructorInfo ci = typeof( VirtualFilePath ).GetConstructor
					(
						new Type[]
						{
							typeof( string ),
							typeof( string ),
							typeof( Boolean )
						}
					);

					VirtualFilePath t = ( VirtualFilePath )value;

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
				VirtualFilePath vfp = VirtualFilePath.Parse
				(
					data.ToString(),
					true,		// Just 'throwOnInvalidPath' can be true, since file paths or
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

			VirtualFilePath other = obj as VirtualFilePath;

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
		/// <param name="filePath">
		/// Can be a local path (in which case the local machine is the host) or "MACHINENAME,c:\local\path"
		/// </param>
		/// <param name="throwOnInvalidPath">
		/// throw exception if the path is not syntactically valid
		/// </param>
		/// <param name="throwIfNotDirectoryPath">throw exception is path is to a file</param>
		/// <param name="throwOnNotExists">throw exception if the directory or file does not exist</param>
		/// <returns>A VirtualFilePath object</returns>
		public static VirtualFilePath Parse
		(
			string filePath,
			Boolean throwOnInvalidPath = false,
			Boolean throwOnNotExists = false )
		{
			string hostName = System.Environment.MachineName;
			string localPath = null;

			#region Validate

			if( string.IsNullOrWhiteSpace( filePath ) )
			{
				throw new ArgumentNullException( nameof( filePath ) );
			}
			else if( filePath.StartsWith( @"\\" ) )
			{
				throw new fr.ScarabException( "UNC paths are not supported" );
			}

			if( filePath.Contains( "," ) )
			{
				var split = filePath.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries );
				hostName = split[ 0 ];
				localPath = System.IO.Path.GetFullPath( split[ 1 ] );
			}
			else
			{
				localPath = System.IO.Path.GetFullPath( filePath );
			}

			if( !CommonRegex.LocalFsoPathRegex.IsMatch( localPath ) )
			{
				if( throwOnInvalidPath )
				{
					throw new ScarabException( "Failed to parse path '{0}'", filePath );
				}
				else
				{
					return null;
				}
			}

			#endregion

			VirtualFilePath ret = new VirtualFilePath
			(
				hostName,
				localPath,
				throwIfNotFilePath: throwOnInvalidPath,
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

		#endregion

		#region Spare Code

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

		///// <summary>
		///// Calculates a path using local or remote syntax depending on caller relative to the server
		///// </summary>
		//public string Path
		//{
		//	get
		//	{
		//		// Allow for the SQL Server localhost moniker
		//		Boolean isLocal = ( 0 == string.Compare( this.HostName, Environment.MachineName, true )
		//								|| 0 == string.Compare( this.HostName, "(local)", true ) );

		//		if( isLocal )
		//		{
		//			Debug.Assert( this.LocalPath[ 1 ] == ':' );
		//			return this.LocalPath;
		//		}
		//		else
		//		{
		//			return this.RemotePath;
		//		}
		//	}
		//}

		//public string LocalPath
		//{
		//	get { return this.localPath; }
		//}

		//public string RemotePath
		//{
		//	get { return this.remotePath; }
		//}

		//public string RelativePath
		//{
		//	get { return this.localPath.Substring( 2 ); }
		//}

		///// <summary>
		///// DNS or NetBIOS name of the machine hosting the directory
		///// </summary>
		//public string HostName
		//{
		//	get { return this.hostName; }
		//}

		//public DriveLetter DriveLetter
		//{
		//	get { return this.driveLetter; }
		//}

		///// <summary>
		///// Returns TRUE if this VirtualFilePath is to a file, FALSE if it's a directory
		///// </summary>
		//public Boolean IsFile
		//{
		//	get { return CommonRegex.ValidFilePathRegex.IsMatch( this.LocalPath ); }
		//}

		//public Boolean IsReadOnly
		//{
		//	get
		//	{
		//		Boolean isReadOnly = true;

		// if( this.IsFile ) { FileAttributes attributes = File.GetAttributes( this.Path );
		// isReadOnly = ( attributes & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly; } else
		// { throw new NotImplementedException( "ReadOnly bit for directory not implmented" ); }

		//		return isReadOnly;
		//	}
		//}

		//public DirectoryInfo AsDirectoryInfo
		//{
		//	get
		//	{
		//		DirectoryInfo ret = null;

		// if( !this.IsFile ) { ret = new DirectoryInfo( this.Path ); }

		//		return ret;
		//	}
		//}
		///// <summary>
		///// Whether the directory or file exists
		///// </summary>
		//public Boolean Exists
		//{
		//	get
		//	{
		//		if( this.IsFile )
		//		{
		//			return File.Exists( this.Path );
		//		}
		//		else
		//		{
		//			return Directory.Exists( this.Path );
		//		}
		//	}
		//}

		#endregion
	}
}