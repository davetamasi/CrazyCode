using System.Text.RegularExpressions;

namespace Tamasi.Shared.WinFramework
{
	/// <summary>
	/// Regular expressions to match common strings used by Windows OS and build process
	/// </summary>
	public static class CommonRegex
	{
		/// <summary>
		/// Used to match the source depot label syntax
		/// "[branch]_[BuildNumber]_[BuildQfe]_[BuildRevision]" (e.g.,
		/// "winblue_gdr_9600_16442_131022-1819")
		/// see http://windowssites/sites/winbuilddocs/Wiki%20Pages/FindBuild%20Web%20Service.aspx
		/// </summary>
		public static readonly Regex BuildLabelRegex = new Regex
		(
			@"^(\w+(?:_[a-z][a-z0-9]+)*)_(\d{4,5})_(\d{1,5})_(\d{6}-\d{4})$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		/// <summary>
		/// Used to match the build lab Official Build Name syntax,
		/// version.qfe.flavor.branch.revision (e.g, 5456.0.amd64fre.vbl_tools_build.060614-1215);
		/// see http://windowssites/sites/winbuilddocs/Wiki%20Pages/FindBuild%20Web%20Service.aspx
		/// </summary>
		public static readonly Regex BuildNameRegex = new Regex
		(
			@"^(\d{4,5})\.(\d{1,5})\.([a-z0-9-_]+)\.([0-9-]+)$", // TODO Pri 2: More precise?
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);



		/// <summary>
		/// Matches a correctly-formed canonical file path that starts with the depot name
		/// (e.g., "minkernel\src\foo\telemetry.c"); Group 1 = dirpath, Group 2 = depotname,
		/// Group 3 = filename
		/// </summary>
		public static readonly Regex CanonicalFilePathRegex = new Regex
		(
			@"^(([^\\/:*?""<>|\r\n]+)(?:\\[^\\/:*?""<>|\r\n]+)*\\)([^\\/:*?""<>|\r\n]+)$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		/// <summary>
		/// Matches a correctly-formed depot file path (e.g., "//depot/[branchname]/[depotname]/dir/dir/file.c")
		/// </summary>
		public static readonly Regex DepotFilePathRegex = new Regex
		(
			@"^//depot/([^/]+)/([^/]+)/(.*)$", // TODO Pri 2: Use the syntax from CanonicalFilePathRegex
			RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled
		);

		/// <summary>
		/// Matches a correctly-formed TFS Area Path that starts with the TeamProject
		/// (e.g., "OS\CORE-OS Core\EnS-Enterprise and Security\SecAssure-Security Assurance\Tooling and Infrastructure")
		/// </summary>
		public static readonly Regex AreaPathRegex = new Regex
		(
			@"^\w+(?:\\[^\\]+)+|\\$",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		/// <summary>
		/// Match any filename(s) with the given extensions in the string
		/// </summary>
		public static readonly Regex ExecutableBinaryNameRegex = new Regex
		(
			@"([a-z0-9\.]{1,254}\.(?:exe|dll|ocx|ax|cpl))",
			RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline
		);

		/// <summary>
		/// Matches a correctly-formed SDDL string
		/// </summary>
		public static readonly Regex SddlStringRegex = new Regex
		(
			@"^(O:(?'owner'[A-Z]+?|S(-[0-9]+)+)?)?(G:(?'group'[A-Z]+?|S(-[0-9]+)+)?)?(D:(?'dacl'[A-Z]*(\([^\)]*\))*))?(S:(?'sacl'[A-Z]*(\([^\)]*\))*))?$",
			RegexOptions.Singleline | RegexOptions.Compiled
		);

		public static readonly Regex FailureBucketRegex = new Regex
		(
			@"(.*)!(.*)",
			RegexOptions.Compiled
		);

		/// <summary>
		/// Matches the canonical form of alias (e.g., "billg" or "b-gang")
		/// </summary>
		public static readonly Regex EmployeeAliasRegex = new Regex
		(
			@"^[A-Z-]{3,8}$",
			RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase
		);

		/// <summary>
		/// Matches the canonical form "Full Name (alias)"
		/// </summary>
		public static readonly Regex EmployeeNameRegex = new Regex
		(
			@"^([\s\p{L}'-]+)$",
			RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase
		);

		/// <summary>
		/// Matches the canonical form "Full Name (alias)"
		/// </summary>
		public static readonly Regex EmployeeStringRegex = new Regex
		(
			@"^([\s\p{L}'-]+)\s\(([a-z-]{3,8})\)$",
			RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase
		);
	}
}