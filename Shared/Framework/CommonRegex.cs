using System.Text.RegularExpressions;

namespace Tamasi.Shared.Framework
{
	public static class CommonRegex
	{
		/// <summary>
		/// Matches any whitespace
		/// </summary>
		public static readonly Regex WhiteSpaceRegex = new Regex( @"\s+" );

		public static readonly Regex GuidRegex = new Regex
		(
			"^.*{([a-f0-9-]{36})}.*$",
			RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled
		);

		/// <summary>
		/// Matches a valid URL
		/// </summary>
		public static readonly Regex UrlRegex = new Regex
		(
			@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*(?<!\.)",
			RegexOptions.Compiled
		);

		#region FileSystem Regexes

		/// <summary>
		/// Matches a filename or dirname (e.g., 'file.c' or 'dirname') or a relative file/dir
		/// path (e.g., 'foo\bar\file.c' or 'foo\bar\dir')
		/// </summary>
		public static readonly Regex RelativePathRegex = new Regex
		(
			@"^(?:[^\\/:*?""<>|\r\n]+\\)*[^\\/:*?""<>|\r\n]{1,260}$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace
		);

		/// <summary>
		/// Matches legal windows local FSO paths (e.g., 'c:\Foo\Bar' or 'c:\Foo\Bar\File.c')
		/// </summary>
		public static readonly Regex LocalFsoPathRegex = new Regex
		(
			@"^[a-z]:\\.*$", // TODO Pri 1: fix the rest
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		/// <summary>
		/// Matches legal windows UNC FSO paths (e.g., '\\MACHINE\Foo\Bar' or
		/// '\\MACHINE\Foo\Bar\Foo\Bar\File.c' or '\\MACHINE\c$\...')
		/// </summary>
		public static readonly Regex UncFsoPathRegex = new Regex
		(
			@".+\..+$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		/// <summary>
		/// Matches legal windows UNC FSO paths (e.g., '\\MACHINE\c$\...')
		/// </summary>
		public static readonly Regex UncPathWithDriveDollarRegex = new Regex
		(
			@"\\{2}([a-zA-Z0-9_-]+)\\([a-zA-Z]\$.*)",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);


		#endregion
	}
}