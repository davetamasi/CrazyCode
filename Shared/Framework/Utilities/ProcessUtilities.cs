using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Tamasi.Shared.Framework
{
	public static class ProcessUtilities
	{
		/// <summary>
		/// Launches the indicated executable in the indicated folder
		/// and pipes its output to Console
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="executableName"></param>
		/// <returns>The process's exit code</returns>
		public static Int32 RunProcessInline
		(
			Environment.SpecialFolder folder,
			string executableName,
			string argumentString,
			DirectoryInfo workingDirectory = null,
			Dictionary<string, string> environmentVars = null,
			Boolean loadUserProfile = false,
			string domainName = null,
			string userName = null,
			string password = null )
		{
			string pathToExe = Path.Combine
			(
				Environment.GetFolderPath( folder ),
				executableName
			);

			return RunProcessInline
			(
				pathToExe,
				argumentString,
				workingDirectory,
				environmentVars,
				loadUserProfile,
				domainName,
				userName,
				password
			);
		}

		/// <summary>
		/// Launches the indicated executable and pipes its output to Console
		/// </summary>
		/// <param name="pathToExe"></param>
		/// <param name="argumentString"></param>
		/// <returns>The process's exit code</returns>
		public static Int32 RunProcessInline
		(
			string pathToExe,
			string argumentString,
			DirectoryInfo workingDirectory = null,
			Dictionary<string, string> environmentVars = null,
			Boolean loadUserProfile = false,
			string domainName = null,
			string userName = null,
			string password = null )
		{
			if( !File.Exists( pathToExe ) )
			{
				throw new ScarabException( "Invalid pathToExe: '{0}'", pathToExe );
			}

			Common.WriteVerboseLine( "BEGIN RunProcessInline: {0} {1}", pathToExe, argumentString );

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = pathToExe;
			psi.Arguments = argumentString;

			// Configure for output redirection
			// http://msdn.microsoft.com/en-us/library/system.diagnostics.processstartinfo.useshellexecute(v=vs.110).aspx

			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;
			psi.LoadUserProfile = loadUserProfile;

			if( workingDirectory != null && workingDirectory.Exists )
			{
				psi.WorkingDirectory = workingDirectory.FullName;
			}

			if( environmentVars != null && environmentVars.Count > 0 )
			{
				/*
				From http://stackoverflow.com/questions/13255745/pass-custom-environment-variables-to-system-diagnostics-process

				Although you cannot set the EnvironmentVariables property, you can modify the
				StringDictionary returned by the property. You must set the UseShellExecute property
				to false to start the process after changing the EnvironmentVariables property. If
				UseShellExecute is true, an InvalidOperationException is thrown when the Start method is called.
				*/

				foreach( KeyValuePair<string, string> kvp in environmentVars )
				{
					if( psi.EnvironmentVariables.ContainsKey( kvp.Key ) )
					{
						psi.EnvironmentVariables[ kvp.Key ] = kvp.Value;
					}
					else
					{
						psi.EnvironmentVariables.Add( kvp.Key, kvp.Value );
					}
				}
			}

			if( userName != null && password != null )
			{
				SecureString ss = new SecureString();

				for( int x = 0; x < password.Length; x++ )
				{
					ss.AppendChar( password[ x ] );
				}

				psi.Domain = domainName;
				psi.UserName = userName;
				psi.Password = ss;
			}

			Int32 exitCode = -1337;

			using( Process process = Process.Start( psi ) )
			{
				using( StreamReader reader = process.StandardOutput )
				{
					string result = reader.ReadToEnd();
					Console.Write( result );
				}

				// Do not wait, as the parent Console takes all the output until
				// the launched process exits
				exitCode = process.ExitCode;
			}

			Common.WriteVerboseLine
			(
				"COMPLETE RunProcessInline: {0} {1} [Exit code {2}]",
				pathToExe,
				argumentString,
				exitCode
			);

			return exitCode;
		}

		/// <summary>
		/// Launches the indicated executable in the indicated folder
		/// and waits for it to complete
		/// </summary>
		/// <param name="folder"></param>
		/// <param name="executableName"></param>
		/// <returns>The process's exit code</returns>
		public static Int32 RunProcessWithWait
		(
			Environment.SpecialFolder folder,
			string executableName,
			string argumentString,
			DirectoryInfo workingDirectory = null,
			Dictionary<string, string> environmentVars = null,
			Boolean loadUserProfile = false,
			string domainName = null,
			string userName = null,
			string password = null )
		{
			string pathToExe = Path.Combine
			(
				Environment.GetFolderPath( folder ),
				executableName
			);

			return RunProcessWithWait
			(
				pathToExe,
				argumentString,
				workingDirectory,
				environmentVars,
				loadUserProfile,
				domainName,
				userName,
				password
			);
		}

		/// <summary>
		/// Launches the indicated executable and waits for it to complete
		/// </summary>
		/// <param name="pathToExe"></param>
		/// <param name="argumentString"></param>
		/// <returns>The process's exit code</returns>
		public static Int32 RunProcessWithWait
		(
			string pathToExe,
			string argumentString,
			DirectoryInfo workingDirectory = null,
			Dictionary<string, string> environmentVars = null,
			Boolean loadUserProfile = false,
			string domainName = null,
			string userName = null,
			string password = null )
		{
			if( !File.Exists( pathToExe ) )
			{
				throw new ScarabException( "Invalid pathToExe: '{0}'", pathToExe );
			}

			Common.WriteVerboseLine( "BEGIN RunProcessWithWait: {0} {1}", pathToExe, argumentString );

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = pathToExe;
			psi.Arguments = argumentString;
			psi.UseShellExecute = false;
			psi.LoadUserProfile = loadUserProfile;

			if( userName != null && password != null )
			{
				SecureString ss = new SecureString();

				for( int x = 0; x < password.Length; x++ )
				{
					ss.AppendChar( password[ x ] );
				}

				psi.Domain = domainName;
				psi.UserName = userName;
				psi.Password = ss;
			}

			if( workingDirectory != null && workingDirectory.Exists )
			{
				psi.WorkingDirectory = workingDirectory.FullName;
			}

			if( environmentVars != null && environmentVars.Count > 0 )
			{
				/*
				From http://stackoverflow.com/questions/13255745/pass-custom-environment-variables-to-system-diagnostics-process

				Although you cannot set the EnvironmentVariables property, you can modify the
				StringDictionary returned by the property. You must set the UseShellExecute property
				to false to start the process after changing the EnvironmentVariables property. If
				UseShellExecute is true, an InvalidOperationException is thrown when the Start method is called.
				*/

				foreach( KeyValuePair<string, string> kvp in environmentVars )
				{
					if( psi.EnvironmentVariables.ContainsKey( kvp.Key ) )
					{
						psi.EnvironmentVariables[ kvp.Key ] = kvp.Value;
					}
					else
					{
						psi.EnvironmentVariables.Add( kvp.Key, kvp.Value );
					}
				}
			}

			if( userName != null && password != null )
			{
				SecureString ss = new SecureString();

				for( int x = 0; x < password.Length; x++ )
				{
					ss.AppendChar( password[ x ] );
				}

				psi.Domain = domainName;
				psi.UserName = userName;
				psi.Password = ss;
			}

			Int32 exitCode = -1337;

			using( Process process = Process.Start( psi ) )
			{
				process.WaitForExit();
				exitCode = process.ExitCode;
			}

			Common.WriteVerboseLine
			(
				"COMPLETE RunProcessWithWait: {0} {1} [Exit code {2}]",
				pathToExe,
				argumentString,
				exitCode
			);

			return exitCode;
		}

		/// <summary>
		/// Launches the indicated executable and sends output as an out param
		/// </summary>
		/// <param name="pathToExe"></param>
		/// <param name="argumentString"></param>
		/// <param name="output"></param>
		/// <returns>The process's exit code</returns>
		public static Int32 RunProcessAndGetOutput
		(
			string pathToExe,
			string argumentString,
			out ReadOnlyCollection<string> output )
		{
			if( !File.Exists( pathToExe ) )
			{
				throw new ScarabException( "Invalid pathToExe: '{0}'", pathToExe );
			}

			List<string> temp = new List<string>();
			const Int32 maxWaitInMilliseconds = 30000;
			Int32 exitCode = -1337;

			using( Process process = new Process() )
			{
				process.StartInfo.FileName = string.Format( "\"{0}\"", pathToExe );
				process.StartInfo.Arguments = argumentString;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.Start();
				process.WaitForExit( maxWaitInMilliseconds );
				StreamReader sr = process.StandardOutput;

				while( !sr.EndOfStream )
				{
					string outputLine = sr.ReadLine().Trim();
					temp.Add( outputLine );
				}

				exitCode = process.ExitCode;
			}

			output = temp.AsReadOnly();

			Common.WriteVerboseLine
			(
				"RunProcessAndGetOutput complete: {0} {1} [Exit code {2}]",
				pathToExe,
				argumentString,
				exitCode
			);

			return exitCode;
		}

		public static Task<Int32> RunProcessAsync
		(
			Environment.SpecialFolder folder,
			string executableName,
			string argumentString,
			DirectoryInfo workingDirectory = null,
			Dictionary<string, string> environmentVars = null,
			Boolean loadUserProfile = false,
			string domainName = null,
			string userName = null,
			string password = null )
		{
			string pathToExe = Path.Combine
			(
				Environment.GetFolderPath( folder ),
				executableName
			);

			return RunProcessAsync
			(
				pathToExe,
				argumentString,
				workingDirectory,
				environmentVars,
				loadUserProfile,
				domainName,
				userName,
				password
			);
		}

		public static Task<Int32> RunProcessAsync
		(
			string pathToExe,
			string argumentString,
			DirectoryInfo workingDirectory = null,
			Dictionary<string, string> environmentVars = null,
			Boolean loadUserProfile = false,
			string domainName = null,
			string userName = null,
			string password = null )
		{
			if( !File.Exists( pathToExe ) )
			{
				throw new ScarabException( "Invalid pathToExe: '{0}'", pathToExe );
			}

			Common.WriteVerboseLine( "BEGIN RunProcessAsync: {0} {1}", pathToExe, argumentString );

			var tcs = new TaskCompletionSource<Int32>();

			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = pathToExe;
			psi.Arguments = argumentString;
			psi.UseShellExecute = false;
			psi.LoadUserProfile = loadUserProfile;

			if( workingDirectory != null && workingDirectory.Exists )
			{
				psi.WorkingDirectory = workingDirectory.FullName;
			}

			if( environmentVars != null && environmentVars.Count > 0 )
			{
				/*
				From http://stackoverflow.com/questions/13255745/pass-custom-environment-variables-to-system-diagnostics-process

				Although you cannot set the EnvironmentVariables property, you can modify the
				StringDictionary returned by the property. You must set the UseShellExecute property
				to false to start the process after changing the EnvironmentVariables property. If
				UseShellExecute is true, an InvalidOperationException is thrown when the Start method is called.
				*/

				foreach( KeyValuePair<string, string> kvp in environmentVars )
				{
					if( psi.EnvironmentVariables.ContainsKey( kvp.Key ) )
					{
						psi.EnvironmentVariables[ kvp.Key ] = kvp.Value;
					}
					else
					{
						psi.EnvironmentVariables.Add( kvp.Key, kvp.Value );
					}
				}
			}

			var process = new Process
			{
				StartInfo = psi,
				EnableRaisingEvents = true
			};

			process.Exited += ( sender, args ) =>
			{
				tcs.SetResult( process.ExitCode );
				process.Dispose();
			};

			process.Start();

			return tcs.Task;
		}
	}
}