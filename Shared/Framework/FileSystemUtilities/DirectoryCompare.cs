using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Threading;
using System.Threading.Tasks;

using fr = Tamasi.Shared.Framework;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	public class DirectoryCompare
	{
		#region Privates and Constructors

		private readonly Lazy<List<DirectoryInfo>> sourceDirs;
		private readonly Lazy<List<DirectoryInfo>> targetDirs;


		public DirectoryCompare( DirectoryInfo sourceDir, DirectoryInfo targetDir )
		{
			this.sourceDirs = new Lazy<List<DirectoryInfo>>( () =>
			{
				return sourceDir.EnumerateDirectories( "*", SearchOption.AllDirectories ).ToList();
			} );

			this.targetDirs = new Lazy<List<DirectoryInfo>>( () =>
			{
				return targetDir.EnumerateDirectories( "*", SearchOption.AllDirectories ).ToList();
			} );
		}

		#endregion

		#region Properties

		public IList<DirectoryInfo> OnlyInSource
		{
			get { return this.sourceDirs.Value.Except( this.targetDirs.Value ).ToList(); }
		}

		public IList<DirectoryInfo> InBoth
		{
			get { return this.sourceDirs.Value.Intersect( this.targetDirs.Value ).ToList(); }

		}
		public IList<DirectoryInfo> OnlyInTarget
		{
			get { return this.targetDirs.Value.Except( this.sourceDirs.Value ).ToList(); }

		}


		#endregion


		#region Statics

		public static Boolean AreSame( VirtualDirectoryPath vpath1, VirtualDirectoryPath vpath2, Boolean verbose )
		{
			DirectoryInfo directory1 = new DirectoryInfo( vpath1.Path );
			DirectoryInfo directory2 = new DirectoryInfo( vpath2.Path );

			return AreSame( directory1, directory2, verbose );
		}

		public static Boolean AreSame( string directoryPath1, string directoryPath2, Boolean verbose )
		{
			DirectoryInfo directory1 = new DirectoryInfo( directoryPath1 );
			DirectoryInfo directory2 = new DirectoryInfo( directoryPath2 );

			return AreSame( directory1, directory2, verbose );
		}

		/// <summary>
		/// Returns true if the directories have the same content, false otherwise
		/// </summary>
		/// <param name="directory1"></param>
		/// <param name="directory2"></param>
		/// <param name="verbose">Whether to print to the command line</param>
		/// <returns></returns>
		public static Boolean AreSame( DirectoryInfo directory1, DirectoryInfo directory2, Boolean verbose )
		{
			if( !Directory.Exists( directory1.FullName ) )
			{
				if( verbose )
				{
					Console.WriteLine( "--> Directory {0} does not exist, comparison failed", directory1.FullName );
				};
				return false;
			}

			if( !Directory.Exists( directory2.FullName ) )
			{
				if( verbose )
				{
					Console.WriteLine( "--> Directory {0} does not exist, comparison failed", directory2.FullName );
				};
				return false;
			}

			// Take a snapshot of the file system
			IEnumerable<System.IO.FileInfo> list1 = directory1.GetFiles( "*.*", System.IO.SearchOption.AllDirectories );
			IEnumerable<System.IO.FileInfo> list2 = directory2.GetFiles( "*.*", System.IO.SearchOption.AllDirectories );

			//A custom file comparer defined below
			FileCompare myFileCompare = new FileCompare();

			// This query determines whether the two folders contain
			// identical file lists, based on the custom file comparer
			// that is defined in the FileCompare class.
			// The query executes immediately because it returns a Boolean.
			Boolean areIdentical = list1.SequenceEqual( list2, myFileCompare );

			if( areIdentical == true )
			{
				fr.Common.WriteVerboseLine
				(
					"Directories '{0}' and '{1}' are identical",
					directory1.FullName,
					directory2.FullName
				);
			}
			else
			{
				if( verbose )
				{
					Console.WriteLine( "--------------------------------------------" );
					fr.Common.WriteVerboseLine( "Directories not the same.  Files:" );
					Console.WriteLine();
					// Find the common files. It produces a sequence and doesn't
					// execute until the foreach statement.
					var queryCommonFiles = list1.Intersect( list2, myFileCompare );

					Console.WriteLine( "--> In both folders:" );
					foreach( var v in queryCommonFiles )
					{
						Console.WriteLine( "    {0}", v.FullName ); //shows which items end up in result list
					}
					Console.WriteLine();

					// Find the set difference between the two folders.
					var queryList1Only = ( from file in list1
										   select file ).Except( list2, myFileCompare );

					Console.WriteLine( "--> Only in {0}:", directory1.FullName );
					foreach( var v in queryList1Only )
					{
						Console.WriteLine( "    {0}", v.FullName );
					}
					Console.WriteLine();

					var queryList2Only = ( from file in list2
										   select file ).Except( list1, myFileCompare );

					Console.WriteLine( "--> The following files are only in {0}:", directory2.FullName );
					foreach( var v in queryList2Only )
					{
						Console.WriteLine( "    {0}", v.FullName );
					}
					Console.WriteLine( "--------------------------------------------" );
				}
			}

			return areIdentical;
		}

		#endregion
	}
}