using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	public static class FileSync
	{
		#region Directory Copies

		public static void CopyDirectoryTree2
		(
			VirtualDirectoryPath sourceDirectory,
			VirtualDirectoryPath targetDirectory,
			CollisionRemediation cr,
			Boolean deleteSourceDirectory = false )
		{
			CopyDirectoryTree2
			(
				sourceDirectory.Path,
				targetDirectory.Path,
				cr,
				deleteSourceDirectory
			);
		}

		/// <summary>
		/// Uses robocopy to copy one directory tree to another
		/// </summary>
		/// <param name="sourceDirectory">Source directory (this is typically a DirectoryInfo not VFP since we don't control it)</param>
		/// <param name="targetDirectory">Directory node to where the source directory tree should be copied</param>
		/// <param name="cr">How to handle collisions</param>
		/// <param name="deleteSourceDirectory">Remove source directory</param>
		public static void CopyDirectoryTree2
		(
			string sourceDirPath,
			string targetDirPath,
			CollisionRemediation cr,
			Boolean deleteSourceDirectory = false )
		{
			Debug.Assert( sourceDirPath != null );
			Debug.Assert( targetDirPath != null );

			if( !Directory.Exists( sourceDirPath ) )
			{
				throw new ScarabException( "Source directory '{0}' does not exist", sourceDirPath );
			}

			string copyJob = string.Format
			(
				"FileSync.CopyDirectoryTree2: {0} {1} {2} {3}",
				sourceDirPath,
				targetDirPath,
				cr.ToString(),
				deleteSourceDirectory
			);
			Debug.WriteLine( copyJob );

			string switches = "/E "; // TODO Pri 2: /MT "; // Multi-threaded, copy full directory tree
			Boolean throwEx = false;

			switch( cr )
			{
				case CollisionRemediation.Break:
				case CollisionRemediation.Passive:
				case CollisionRemediation.FillIn: // TODO Pri 2: Figure out robocopy switches for these
					break;

				case CollisionRemediation.Mirror:
					switches = "/MIR ";
					break;

				case CollisionRemediation.Overwrite:
					break;

				case CollisionRemediation.Throw:
					throwEx = true;
					break;
			}

			if( Directory.Exists( targetDirPath ) && throwEx )
			{
				throw new ScarabException( "Target directory '{0}' already exists", targetDirPath );
			}

			if( deleteSourceDirectory ) switches += "/MOVE ";

			string robocopyArgs = string.Format
			(
				"\"{0}\" \"{1}\" {2}",
				sourceDirPath,
				targetDirPath,
				switches
			);

			if( !File.Exists( Constants.ROBOCOPY_EXE_PATH ) )
			{
				throw new ScarabException( "Invalid robocopy path" );
			}

			for( Int32 k=0; k <= 3; k++ )
			{
				Thread.Sleep( 1000 );

				Int32 ret = ProcessUtilities.RunProcessInline
				(
					Constants.ROBOCOPY_EXE_PATH,
					robocopyArgs
				);

				// Int32 ret = ProcessUtilities.RunProcessWithWait( robocopyPath, robocopyArgs );

				if( ret != 16 )
				{
					Common.WriteLine( "Robocopy result: {0}", CalculateRobocopyResult( ret ) );
					Console.WriteLine( "------------------------------------------------------------------------------" );
					break;
				}
				else if( k == 3 )
				{
					throw new ScarabException( "Fatal Robocopy Error on {0}", copyJob );
				}

				Common.DrawWarning( "Robocopy result: {0}, retrying...", CalculateRobocopyResult( ret ) );
			}
		}

		public static async Task CopyDirectoryTreeAsync
		(
			VirtualDirectoryPath sourceDirectory,
			VirtualDirectoryPath targetDirectory,
			CollisionRemediation cr,
			Boolean deleteSourceDirectory = false )
		{
			await CopyDirectoryTreeAsync
			(
				sourceDirectory.Path,
				targetDirectory.Path,
				cr,
				deleteSourceDirectory
			);
		}

		/// <summary>
		/// Uses robocopy asynchronously to copy one directory tree to another
		/// </summary>
		/// <param name="sourceDirectory">Source directory (this is typically a DirectoryInfo not VFP since we don't control it)</param>
		/// <param name="targetDirectory">Directory node to where the source directory tree should be copied</param>
		/// <param name="cr">How to handle collisions</param>
		/// <param name="deleteSourceDirectory">Remove source directory</param>
		public static async Task CopyDirectoryTreeAsync
		(
			string sourceDirPath,
			string targetDirPath,
			CollisionRemediation cr,
			Boolean deleteSourceDirectory = false )
		{
			Debug.Assert( sourceDirPath != null );
			Debug.Assert( targetDirPath != null );

			if( !Directory.Exists( sourceDirPath ) )
			{
				throw new ScarabException( "Source directory '{0}' does not exist", sourceDirPath );
			}

			string copyJob = string.Format
			(
				"FileSync.CopyDirectoryTree2: {0} {1} {2} {3}",
				sourceDirPath,
				targetDirPath,
				cr.ToString(),
				deleteSourceDirectory
			);
			Debug.WriteLine( copyJob );

			string switches = "/E "; // TODO Pri 2: /MT "; // Multi-threaded, copy full directory tree
			Boolean throwEx = false;

			switch( cr )
			{
				case CollisionRemediation.Break:
				case CollisionRemediation.Passive:
				case CollisionRemediation.FillIn: // TODO Pri 2: Figure out robocopy switches for these
					break;

				case CollisionRemediation.Mirror:
					switches = "/MIR ";
					break;

				case CollisionRemediation.Overwrite:
					break;

				case CollisionRemediation.Throw:
					throwEx = true;
					break;
			}

			if( Directory.Exists( targetDirPath ) && throwEx )
			{
				throw new ScarabException( "Target directory '{0}' already exists", targetDirPath );
			}

			if( deleteSourceDirectory ) switches += "/MOVE ";

			string robocopyArgs = string.Format
			(
				"\"{0}\" \"{1}\" {2}",
				sourceDirPath,
				targetDirPath,
				switches
			);

			if( !File.Exists( Constants.ROBOCOPY_EXE_PATH ) )
			{
				throw new ScarabException( "Invalid robocopy path" );
			}

			for( Int32 k=0; k <= 3; k++ )
			{
				Int32 ret = await ProcessUtilities.RunProcessAsync
				(
					Constants.ROBOCOPY_EXE_PATH,
					robocopyArgs
				);

				// Int32 ret = ProcessUtilities.RunProcessWithWait( robocopyPath, robocopyArgs );

				if( ret != 16 )
				{
					Common.WriteLine( "Robocopy result: {0}", CalculateRobocopyResult( ret ) );
					Console.WriteLine( "------------------------------------------------------------------------------" );
					break;
				}
				else if( k == 3 )
				{
					throw new ScarabException( "Fatal Robocopy Error on {0}", copyJob );
				}

				Common.DrawWarning( "Robocopy result: {0}, retrying...", CalculateRobocopyResult( ret ) );
			}
		}

		#endregion

		#region File Copies

		public static void CopyFile
		(
			VirtualFilePath sourceFile,
			VirtualFilePath targetFile,
			CollisionRemediation cr )
		{
			CopyFileWorker( sourceFile.Path, targetFile.Path, cr );
		}

		public static void CopyFile
		(
			VirtualFilePath sourceFile,
			string targetFilePath,
			CollisionRemediation cr )
		{
			CopyFileWorker( sourceFile.Path, targetFilePath, cr );
		}

		public static void CopyFile
		(
			FileInfo sourceFile,
			FileInfo targetFile,
			CollisionRemediation cr )
		{
			CopyFileWorker( sourceFile.FullName, targetFile.FullName, cr );
		}

		public static void CopyFile
		(
			string sourceFilePath,
			string targetFilePath,
			CollisionRemediation cr )
		{
			CopyFileWorker( sourceFilePath, targetFilePath, cr );
		}

		/// <summary>
		/// Similar to CopyFile, but engineered for large (>1GB) files, as it
		/// uses a copy marker
		/// </summary>
		public static void CopyLargeFile
		(
			FileInfo sourceFile,
			FileInfo targetFile,
			CollisionRemediation cr )
		{
			// TODO Pri 2 (Perf): Use PowerShell to do the copy directly if sender
			// and receiver machines are not the current machine

			Debug.Assert( sourceFile != null );

			Common.WriteLine
			(
				"BEGIN Copying {0} to {1}",
				sourceFile.FullName,
				targetFile.FullName
			);

			if( File.Exists( targetFile.FullName + Constants.COPY_MARKER ) )
			{
				Common.WriteVerboseLine
				(
					"Deleting existing transfer artifact {0}",
					targetFile.FullName + Constants.COPY_MARKER
				);

				File.Delete( targetFile.FullName + Constants.COPY_MARKER );
			}

			Boolean bail = false;
			Boolean throwex = false;
			Boolean proceed = true;

			if( targetFile.Exists )
			{
				proceed = false;

				switch( cr )
				{
					case CollisionRemediation.Break:
					case CollisionRemediation.Passive:
					case CollisionRemediation.FillIn:
						Common.WriteLine( "Target file exists, exiting" );
						bail = true;
						break;

					case CollisionRemediation.Mirror:
						if( FileCompare.AreSameBinaryViaFullScan( sourceFile, targetFile ) ) // TODO Pri 2 (Perf): Avoid full scan?
						{
							Common.WriteLine( "Target file already exists (and is identical), exiting" );
						}
						else
						{
							Common.WriteLine( "Target file exists, but is different: deleting" );
							File.Delete( targetFile.FullName );
							proceed = true;
						}
						break;

					case CollisionRemediation.Overwrite:
						Common.WriteLine( "Target file exists, deleting" );
						File.Delete( targetFile.FullName );
						proceed = true;
						break;

					case CollisionRemediation.Throw:
						throwex = true;
						break;
				}
			}

			if( throwex )
			{
				throw new ScarabException( "Target file '{0}' already exists", targetFile.FullName );
			}
			else if( proceed )
			{
				Common.WriteVerboseLine
				(
					"Copying {0} to {1}",
					sourceFile.FullName,
					targetFile.FullName + Constants.COPY_MARKER
				);

				// E.g., to \\Path\LARGE_FILE_TEMPLATE.vhd.transferring
				sourceFile.CopyTo( targetFile.FullName + Constants.COPY_MARKER );

				Common.WriteVerboseLine
				(
					"Renaming {0} to {1}",
					targetFile.FullName + Constants.COPY_MARKER,
					targetFile.FullName
				);

				File.Move( targetFile.FullName + Constants.COPY_MARKER, targetFile.FullName );

				Common.WriteLine
				(
					"COMPLETE Copying {0} to {1}",
					sourceFile.FullName,
					targetFile.FullName
				);
			}
			else if( bail )
			{

			}
		}

		#endregion

		#region Private Statics

		/// <summary>
		/// Copies a single file
		/// </summary>
		/// <param name="sourceFile">Source directory node</param>
		/// <param name="targetFile">Directory node to where the source directory tree should be copied</param>
		/// <param name="cr">How to handle collisions</param>
		/// <param name="deleteSourceFile">Remove source file (a.k.a. MOVE)</param>
		private static void CopyFileWorker
		(
			string sourceFilePath,
			string targetFilePath,
			CollisionRemediation cr )
		{
			Debug.Assert( sourceFilePath != null );
			Debug.Assert( targetFilePath != null );

			Common.WriteVerboseLine
			(
				"--> FileSync.CopyFile: {0} {1} {2}",
				sourceFilePath,
				targetFilePath,
				cr.ToString()
			);

			//string switches = "/F /Z /V ";
			Boolean throwEx = false;

			/*
			XCOPY source [destination] [/A | /M] [/D[:date]] [/P] [/S [/E]] [/V] [/W]
									   [/C] [/I] [/Q] [/F] [/L] [/G] [/H] [/R] [/T] [/U]
									   [/K] [/N] [/O] [/X] [/Y] [/-Y] [/Z] [/B] [/J]
									   [/EXCLUDE:file1[+file2][+file3]...]
			  source       Specifies the file(s) to copy.
			  destination  Specifies the location and/or name of new files.
			  /A           Copies only files with the archive attribute set,
						   doesn't change the attribute.
			  /M           Copies only files with the archive attribute set,
						   turns off the archive attribute.
			  /D:m-d-y     Copies files changed on or after the specified date.
						   If no date is given, copies only those files whose
						   source time is newer than the destination time.
			  /EXCLUDE:file1[+file2][+file3]...
						   Specifies a list of files containing strings.  Each string
						   should be in a separate line in the files.  When any of the
						   strings match any part of the absolute path of the file to be
						   copied, that file will be excluded from being copied.  For
						   example, specifying a string like \obj\ or .obj will exclude
						   all files underneath the directory obj or all files with the
						   .obj extension respectively.
			  /P           Prompts you before creating each destination file.
			  /S           Copies directories and subdirectories except empty ones.
			  /E           Copies directories and subdirectories, including empty ones.
						   Same as /S /E. May be used to modify /T.
			  /V           Verifies the size of each new file.
			  /W           Prompts you to press a key before copying.
			  /C           Continues copying even if errors occur.
			  /I           If destination does not exist and copying more than one file,
						   assumes that destination must be a directory.
			  /Q           Does not display file names while copying.
			  /F           Displays full source and destination file names while copying.
			  /L           Displays files that would be copied.
			  /G           Allows the copying of encrypted files to destination that does not support encryption.
			  /H           Copies hidden and system files also.
			  /R           Overwrites read-only files.
			  /T           Creates directory structure, but does not copy files. Does not
						   include empty directories or subdirectories. /T /E includes
						   empty directories and subdirectories.
			  /U           Copies only files that already exist in destination.
			  /K           Copies attributes. Normal Xcopy will reset read-only attributes.
			  /N           Copies using the generated Int16 names.
			  /O           Copies file ownership and ACL information.
			  /X           Copies file audit settings (implies /O).
			  /Y           Suppresses prompting to confirm you want to overwrite an existing destination file.
			  /-Y          Causes prompting to confirm you want to overwrite an existing destination file.
			  /Z           Copies networked files in restartable mode.
			  /B           Copies the Symbolic Link itself versus the target of the link.
			  /J           Copies using unbuffered I/O. Recommended for very large files.

			The switch /Y may be preset in the COPYCMD environment variable.
			This may be overridden with /-Y on the command line.
			*/

			Boolean proceed = true;

			if( File.Exists( targetFilePath ) )
			{
				proceed = false;

				switch( cr )
				{
					case CollisionRemediation.Break:
					case CollisionRemediation.Passive:
					case CollisionRemediation.FillIn:
						Common.WriteLine( "Target file exists, exiting" );
						break;

					case CollisionRemediation.Mirror:
						if( FileCompare.AreSameBinaryViaFullScan
						(
							new FileInfo( sourceFilePath ),
							new FileInfo( targetFilePath ) ) ) // TODO Pri 1: FASTER
						{
							Common.WriteLine( "Target file already exists (and is identical), exiting" );
						}
						else
						{
							Common.WriteLine( "Target file exists, but is different: deleting" );
							File.Delete( targetFilePath );
							proceed = true;
						}
						break;

					case CollisionRemediation.Overwrite:
						Common.WriteLine( "Target file exists, deleting" );
						File.Delete( targetFilePath );
						proceed = true;
						break;

					case CollisionRemediation.Throw:
						throwEx = true;
						break;
				}
			}

			if( throwEx )
			{
				throw new ScarabException( "Target file '{0}' already exists", targetFilePath );
			}

			if( proceed )
			{
				File.Copy( sourceFilePath, targetFilePath );
			}
		}

		private static string CalculateRobocopyResult( Int32 errorlevel )
		{
			string ret = null;

			if( errorlevel == 16 ) ret = " ***FATAL ERROR***";
			else if( errorlevel == 15 ) ret = " OKCOPY + FAIL + MISMATCHES + XTRA";
			else if( errorlevel == 14 ) ret = " FAIL + MISMATCHES + XTRA";
			else if( errorlevel == 13 ) ret = " OKCOPY + FAIL + MISMATCHES";
			else if( errorlevel == 12 ) ret = " FAIL + MISMATCHES";
			else if( errorlevel == 11 ) ret = " OKCOPY + FAIL + XTRA";
			else if( errorlevel == 10 ) ret = " FAIL + XTRA";
			else if( errorlevel == 9 ) ret = " OKCOPY + FAIL";
			else if( errorlevel == 8 ) ret = " FAIL";
			else if( errorlevel == 7 ) ret = " OKCOPY + MISMATCHES + XTRA";
			else if( errorlevel == 6 ) ret = " MISMATCHES + XTRA";
			else if( errorlevel == 5 ) ret = " OKCOPY + MISMATCHES";
			else if( errorlevel == 4 ) ret = " MISMATCHES";
			else if( errorlevel == 3 ) ret = " OKCOPY + XTRA";
			else if( errorlevel == 2 ) ret = " XTRA";
			else if( errorlevel == 1 ) ret = " OKCOPY";
			else if( errorlevel == 0 ) ret = " No Change";

			return ret;
		}

		#endregion

		#region Spare Code

		/*
		public static ReadOnlyCollection<FileInfoEx> GetFiles( string rootPath )
		{
			DirectoryInfoEx root = new DirectoryInfoEx( rootPath );

			List<FileInfoEx> allFiles = new List<FileInfoEx>();
			Stack<DirectoryInfoEx> dirs = new Stack<DirectoryInfoEx>( 20 );
			dirs.Push( root );
			while( dirs.Count > 0 )
			{
				DirectoryInfoEx currentDir = dirs.Pop();
				DirectoryInfoEx[] subDirs = null;

				try
				{
					subDirs = currentDir.GetDirectories();
					allFiles.AddRange( currentDir.GetFiles() );
					// Push the subdirectories onto the stack for traversal.
					if( subDirs != null && subDirs.Length > 0 )
					{
						foreach( DirectoryInfoEx di in subDirs )
							dirs.Push( di );
					}
				}
				// Exception will be thrown if we do not have discovery permission on a folder or file,
				// or a junction folder that is not a real folder, log those folders if this happens
				catch( PathTooLongException e )
				{
					ExceptionLog.Log( string.Format( "Common.GetFiles({0})", root.FullName ), e );
				}
				catch( UnauthorizedAccessException e )
				{
					ExceptionLog.Log( string.Format( "Common.GetFiles({0})", root.FullName ), e );
				}
				catch( System.IO.DirectoryNotFoundException e )
				{
					ExceptionLog.Log( string.Format( "Common.GetFiles({0})", root.FullName ), e );
				}
			}

			return allFiles.AsReadOnly();
		}

		/// <summary>
		/// Compare file based on length and last write time stamp
		/// </summary>
		public static Boolean IsFileSame( FileInfo sourceFile, string targetFilePath )
		{
			if( !File.Exists( targetFilePath ) )
			{
				return false;
			}

			FileInfo targetFile = new FileInfo( targetFilePath );
			return sourceFile.Length == targetFile.Length && sourceFile.LastWriteTime == targetFile.LastWriteTime;
		}

		public static Boolean IsFileSame( FileInfoEx sourceFile, string targetFilePath )
		{
			if( !File.Exists( targetFilePath ) )
			{
				return false;
			}

			FileInfo targetFile = new FileInfo( targetFilePath );

			FileInfo sourceFileWrap = new FileInfo( sourceFile.FullName );

			return sourceFileWrap.Length == targetFile.Length && sourceFileWrap.LastWriteTime == targetFile.LastWriteTime;
		}

		public static void ProcessDelete( IList<CopyFileInfo> copyFiles, out Int32 deleteCount )
		{
			deleteCount = 0;
			try
			{
				List<String> targetDirs = new List<String>();
				List<FileInfo> targetFiles = new List<FileInfo>();
				List<String> copyFileNames = new List<String>();
				String targetDirPath;
				String targetPath;
				foreach( CopyFileInfo file in copyFiles )
				{
					targetPath = file.TargetFilePath;
					targetDirPath = targetPath.Substring( 0, targetPath.LastIndexOf( '\\' ) );
					if( !targetDirs.Contains( targetDirPath, StringComparer.OrdinalIgnoreCase ) )
					{
						targetDirs.Add( targetDirPath );
						targetFiles.AddRange( new DirectoryInfo( targetDirPath ).GetFiles() );
					}
					if( !copyFileNames.Contains( targetPath, StringComparer.OrdinalIgnoreCase ) )
						copyFileNames.Add( targetPath );
				}

				foreach( FileInfo file in targetFiles )
				{
					if( !copyFileNames.Contains( file.FullName, StringComparer.OrdinalIgnoreCase ) )
					{
						file.Delete();
						deleteCount++;
						Console.WriteLine( "{0} deleted", file.Name );
					}
				}
			}
			catch( Exception ex )
			{
				Console.WriteLine( String.Format( "File deletion failed: {0}", ex.Message ) );
			}
		}

		public static void ProcessCopy( IList<CopyFileInfo> copyFiles, Boolean isVerboseOutput, Int32 retryCount, ref Int32[] changeCounts )
		{
			copyFiles = ProcessCopy( copyFiles, isVerboseOutput, ref changeCounts );
			Int32 retry = 1;
			while( copyFiles.Count > 0 && ++retry < retryCount )
			{
				CommonEx.DrawWarning( "Retried failed files for " + retry.ToString() + " time:" );
				copyFiles = ProcessCopy( copyFiles, isVerboseOutput, ref changeCounts );
			}

			if( copyFiles.Count > 0 )
			{
				CommonEx.DrawWarning( "{0} files failed to copy", copyFiles.Count );
				foreach( CopyFileInfo file in copyFiles )
				{
					Console.WriteLine( file.TargetFilePath );
				}
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Runs the copy, returns List of failed copies
		/// </summary>
		private static List<CopyFileInfo> ProcessCopy( IList<CopyFileInfo> filesToCopy, Boolean verbose, ref Int32[] changeCounts )
		{
			if( filesToCopy == null ) return new List<CopyFileInfo>();

			List<CopyFileInfo> failedList = new List<CopyFileInfo>();
			Int32 overwrittenNum = 0;
			Int32 addNum = 0;

			Parallel.ForEach<CopyFileInfo>( filesToCopy, delegate( CopyFileInfo fileToCopy )
			{
				// FileInfo sourceFile = null;

				try
				{
					if( IsFileSame( fileToCopy.SourceFile, fileToCopy.TargetFilePath ) )
					{
						if( verbose )
						{
							Console.WriteLine( "Same {0}", fileToCopy.SourceFile.FullName );
						}
					}
					else
					{
						Boolean targetFileExists = File.Exists( fileToCopy.TargetFilePath );

						if( verbose )
						{
							Console.WriteLine
							(
								"[{0:00}]-> {1} {2}",
								Thread.CurrentThread.ManagedThreadId,
								targetFileExists ? "Overwriting" : "Copying",
								fileToCopy.SourceFile.Name
							);

							//fileToCopy.SourceFile.CopyTo( fileToCopy.TargetFilePath, true );
							FileInfo temp1 = new FileInfo( fileToCopy.SourceFile.FullName );
							temp1.CopyTo( fileToCopy.TargetFilePath, true );

							Console.WriteLine
							(
								"[{0:00}]-> {1} complete",
								Thread.CurrentThread.ManagedThreadId,
								fileToCopy.SourceFile.Name
							);
						}
						else
						{
							//fileToCopy.SourceFile.CopyTo( fileToCopy.TargetFilePath, true );
							FileInfo temp1 = new FileInfo( fileToCopy.SourceFile.FullName );
							temp1.CopyTo( fileToCopy.TargetFilePath, true );

							Debug.WriteLine( "{0} {1}", targetFileExists ? "Overwritten" : "Added", fileToCopy.TargetFilePath );
						}

						if( targetFileExists )
							overwrittenNum++;
						else
							addNum++;
					}
				}
				catch( Exception ex )
				{
					failedList.Add( fileToCopy );

					CommonEx.DrawWarning
					(
						string.Format( "--> Copy {0} failed: {1}", fileToCopy.SourceFile.FullName, ex.Message )
					);
				}
			} );

			Interlocked.Add( ref changeCounts[ 0 ], addNum );
			Interlocked.Add( ref changeCounts[ 1 ], overwrittenNum );

			//changeCounts[ 0 ] += addNum;
			//changeCounts[ 1 ] += overwrittenNum;

			return failedList;
		}

		/// <summary>
		/// Transactionally copies one directory to another
		/// </summary>
		[Obsolete]
		public static void CopyDirectoryTree
		(
			DirectoryInfo source,
			DirectoryInfo target,
			CollisionRemediation cr,
			Boolean deleteSourceDirectory = false )
		{
			Debug.WriteLine( "--> FileSync.CopyDirectoryTree({0},{1})", source.FullName, target.FullName );

			// Wave 3 Pri 2 Formalize difference between CopyDirectoryTree and CopyDirectoryTree2, make sure cr works correctly for each

			if( !Directory.Exists( source.FullName ) )
			{
				throw new ScarabException( "Source directory '{0}' not found", source.FullName );
			}

			Boolean overwriteFiles = true;
			Boolean throwEx = false;

			if( cr != CollisionRemediation.Passive && Directory.Exists( target.FullName ) )
			{
				switch( cr )
				{
					case CollisionRemediation.Break:
						return;
						break;

					case CollisionRemediation.FillIn:
						overwriteFiles = false;
						break;

					case CollisionRemediation.Mirror:
						// Use the big gun instead  Directory.Delete( target.FullName, true );
						Utilities.DeleteTree( target.FullName );
						break;

					case CollisionRemediation.Overwrite:
						// Just continue, overwrite if possible
						break;

					case CollisionRemediation.Throw:
						throwEx = true;
						break;
				}
			}

			if( throwEx )
			{
				throw new ScarabException( "Target directory '{0}' already exists", target.FullName );
			}

			CopyDirectoryTreeWorker( source, target, overwriteFiles );

			if( deleteSourceDirectory )
			{
				Directory.Delete( source.FullName, true );
			}
		}

		private static void CopyDirectoryTreeWorker( DirectoryInfo source, DirectoryInfo target, Boolean overwriteFiles )
		{
			Debug.WriteLine( "--> FileSync.CopyDirectoryTreeWorker({0},{1})", source.FullName, target.FullName );

			// Check if the target directory exists, if not, create it.
			if( !Directory.Exists( target.FullName ) )
			{
				for( Int32 k = 0; k < 32; k++ )
				{
					CommonEx.WriteLine( true, "{0}th attempt to create {1}", k, target.FullName );

					Directory.CreateDirectory( target.FullName );

					if( Directory.Exists( target.FullName ) )
					{
						break;
					}

					Thread.Sleep( 3000 );
				}

				// Trust, but verify
				if( !Directory.Exists( target.FullName ) )
				{
					throw new ScarabException
					(
						"Target directory '{0}' not found and could not be created",
						target.FullName
					);
				}
			}

			// Copy each file into its new directory
			foreach( FileInfo fi in source.GetFiles() )
			{
				Debug.WriteLine( @"--> Copying {0}\{1}", target.FullName, fi.Name );
				string targetPath = Path.Combine( target.ToString(), fi.Name );
#if !DEBUG
				try
				{
#endif
				fi.CopyTo( targetPath, overwriteFiles );
#if !DEBUG
				}
				catch( Exception e )
				{
					CommonEx.DrawWarning
					(
						"--> Error copying file from '{0}' to '{1}': {2}",
						fi.FullName,
						targetPath,
						e.Message
					);
				}
#endif
			}

			// Copy each subdirectory using recursion
			foreach( DirectoryInfo diSourceSubDir in source.GetDirectories() )
			{
				DirectoryInfo nextTargetSubDir =
					target.CreateSubdirectory( diSourceSubDir.Name );
				CopyDirectoryTreeWorker( diSourceSubDir, nextTargetSubDir, overwriteFiles );
			}
		}

		/// <summary>
		/// An instance of a file to copy using Xun's copy functions
		/// </summary>
		public sealed class CopyFileInfo
		{
			private FileInfoEx sourceFile;
			private String targetFilePath;
			private static System.Object checkDirLock = new System.Object();

			public CopyFileInfo( FileInfoEx sourceFile, string targetFilePath )
			{
				this.sourceFile = sourceFile;
				this.targetFilePath = targetFilePath;
				string dirPath = targetFilePath.Substring( 0, targetFilePath.LastIndexOf( "\\", StringComparison.OrdinalIgnoreCase ) );

				lock( checkDirLock )
				{
					if( !Directory.Exists( dirPath ) )
						Directory.CreateDirectory( dirPath );
				}
			}

			public CopyFileInfo( FileInfo sourceFile, string targetFilePath ) : this( new FileInfoEx( sourceFile.FullName ), targetFilePath ) { }

			public FileInfoEx SourceFile
			{
				get
				{
					return sourceFile;
				}
			}

			public String TargetFilePath
			{
				get
				{
					return targetFilePath;
				}
			}
		}
		 * */

		///// <summary>
		///// Smart copy files from source to target
		///// </summary>
		//[Obsolete]
		//public static void SmartCopy( IList<CopyFileInfo> copyFiles, Boolean isVerboseOutput, Int32 retryCounts, ref Int32[] changeCounts )
		//{
		//	ProcessDelete( copyFiles, out changeCounts[ 0 ] );
		//	Int32[] tempCounts = new Int32[ 2 ];
		//	ProcessCopy( copyFiles, true, retryCounts, ref tempCounts );
		//	changeCounts[ 1 ] = tempCounts[ 0 ];
		//	changeCounts[ 2 ] = tempCounts[ 1 ];
		//}

		///// <summary>
		///// Output sync total data
		///// </summary>
		//[Obsolete]
		//public static void OutputSyncTotal( Int32[] changeCounts )
		//{
		//	Console.WriteLine();
		//	Console.WriteLine( "=== TOTAL Summary" );
		//	Console.WriteLine( "Deleted: {0}", changeCounts[ 0 ] );
		//	Console.WriteLine( "Added: {0}", changeCounts[ 1 ] );
		//	Console.WriteLine( "Overwritten: {0}", changeCounts[ 2 ] );
		//	Console.WriteLine( "Total: {0}", changeCounts[ 0 ] + changeCounts[ 1 ] + changeCounts[ 2 ] );
		//	Console.WriteLine( "====================" );
		//}

		///// <summary>
		///// Concurrently create directory, if not currently existing
		///// </summary>
		//public static void CoCreateDirectory( String dirPath )
		//{
		//	do
		//	{
		//		try
		//		{
		//			if( !Directory.Exists( dirPath ) )
		//				Directory.CreateDirectory( dirPath );
		//			else
		//				break;
		//		}
		//		catch( IOException )
		//		{
		//			Thread.Sleep( 1000 );
		//		}
		//	} while( true );
		//}

		///// <summary>
		///// Concurrent file copy
		///// </summary>
		//[Obsolete]
		////public static void CoCopyFile( FileInfo file, String targetFilePath )
		////{
		////	//create directory if not exists
		////	String dirPath = targetFilePath.Substring( 0, targetFilePath.LastIndexOf( @"\", StringComparison.OrdinalIgnoreCase ) );
		////	if( !File.Exists( dirPath ) )
		////	{
		////		CoCreateDirectory( dirPath );
		////	}

		////	while( !IsFileSame( file, targetFilePath ) )
		////	{
		////		try
		////		{
		////			file.CopyTo( targetFilePath, true );
		////			Console.WriteLine( "Copied " + file.Name );
		////			break;
		////		}
		////		catch( IOException e )
		////		{
		////			if( e.Message.IndexOf( "because it is being used by another process", StringComparison.OrdinalIgnoreCase ) > -1 )
		////			{
		////				Console.WriteLine( "wait on " + file.Name );
		////				Thread.Sleep( Settings.INJECTED_LATENTCY );
		////			}
		////		}
		////	};
		////}

		#endregion
	}
}