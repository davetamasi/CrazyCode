using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	public static class AccessControl
	{
		#region Public Statics

		/// <summary>
		/// Recursively grants caller ownership and full control of the indicated directory
		/// </summary>
		/// <param name="path">Absolute path of the directory</param>
		public static void TakeOverDirectory( string path, Boolean recurse )
		{
			if( !Directory.Exists( path ) )
			{
				throw new ArgumentException( "Nonexistent directory" );
			}

			DirectoryInfo root = new DirectoryInfo( path );
			SetFullControl( root );

			// Now apply it everywhere
			if( recurse )
			{
				foreach( DirectoryInfo di in root.GetDirectories( "*", SearchOption.AllDirectories ) )
				{
					SetFullControl( di );
				}
			}
		}

		/// <summary>
		/// Recursively grants caller ownership and full control of the indicated directory
		/// </summary>
		/// <param name="path">Absolute path of the directory</param>
		public static void TakeOverFile( string filePath )
		{
			if( !File.Exists( filePath ) )
			{
				throw new ArgumentException( "Nonexistent file" );
			}

			FileInfo fi = new FileInfo( filePath );
			SetOwner( fi );
			SetFullControl( fi );
		}

		public static void SetOwner( DirectoryInfo di, WindowsIdentity wi = null )
		{
			DirectorySecurity ds = new DirectorySecurity();

			if( wi == null )
			{
				ds.SetOwner( new NTAccount( WindowsIdentity.GetCurrent().Name ) );
			}
			else
			{
				ds.SetOwner( new NTAccount( wi.Name ) );
			}

			ds.SetOwner( new NTAccount( WindowsIdentity.GetCurrent().Name ) );
			di.SetAccessControl( ds );
		}

		public static void SetFullControl( DirectoryInfo di, WindowsIdentity wi = null )
		{
			FileIOPermission f2 = new FileIOPermission( FileIOPermissionAccess.AllAccess, di.FullName );
			f2.Demand();

			String trusteeName = wi != null ? wi.Name : WindowsIdentity.GetCurrent().Name;

			DirectorySecurity ds = new DirectorySecurity();

			FileSystemAccessRule fsar = new FileSystemAccessRule
			(
				trusteeName,
				FileSystemRights.FullControl | FileSystemRights.TakeOwnership,
				InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
				PropagationFlags.None,
				AccessControlType.Allow
			);

			ds.SetAccessRule( fsar );
			di.SetAccessControl( ds );
		}

		/// <summary>
		/// Sets the owner of the given file to the indicated principal, or current user
		/// </summary>
		/// <param name="fi">The target file</param>
		/// <param name="wi">If not null, the principal to become the owner</param>
		public static void SetOwner( FileInfo fi, WindowsIdentity wi = null )
		{
			FileSecurity fs = new FileSecurity();

			if( wi == null )
			{
				fs.SetOwner( new NTAccount( WindowsIdentity.GetCurrent().Name ) );
			}
			else
			{
				fs.SetOwner( new NTAccount( wi.Name ) );
			}

			fi.SetAccessControl( fs );
		}

		public static void SetFullControl( FileInfo fi, WindowsIdentity wi = null )
		{
			FileIOPermission fiop = new FileIOPermission( FileIOPermissionAccess.AllAccess, fi.FullName );
			fiop.Demand();

			String trusteeName = wi != null ? wi.Name : WindowsIdentity.GetCurrent().Name;

			FileSecurity fs = new FileSecurity();

			FileSystemAccessRule fsar = new FileSystemAccessRule
			(
				trusteeName,
				FileSystemRights.FullControl | FileSystemRights.TakeOwnership,
				InheritanceFlags.None,
				PropagationFlags.None,
				AccessControlType.Allow
			);

			fs.SetAccessRule( fsar );
			fi.SetAccessControl( fs );
		}

		#endregion
	}
}