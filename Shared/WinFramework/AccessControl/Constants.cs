using System;

namespace Tamasi.Shared.WinFramework.AccessControl
{
	internal static class Constants
	{
		internal const UInt16 SID_MAX_SUB_AUTHORITIES = 15;

		/// <summary>
		/// Table of well known SID strings
		/// </summary>
		/// <remarks>The table indicies correspond to <see cref="WELL_KNOWN_SID_TYPE"/> s</remarks>
		internal static readonly string[] WellKnownSIDs = new string[]
		{
			"S-1-0-0", // NULL SID
			"S-1-1-0", // Everyone
			"S-1-2-0", // LOCAL
			"S-1-3-0", // CREATOR OWNER
			"S-1-3-1", // CREATOR GROUP
			"S-1-3-2", // CREATOR OWNER SERVER
			"S-1-3-3", // CREATOR GROUP SERVER
			"S-1-5", // NT Pseudo Domain\NT Pseudo Domain
			"S-1-5-1", // NT AUTHORITY\DIALUP
			"S-1-5-2", // NT AUTHORITY\NETWORK
			"S-1-5-3", // NT AUTHORITY\BATCH
			"S-1-5-4", // NT AUTHORITY\INTERACTIVE
			"S-1-5-6", // NT AUTHORITY\SERVICE
			"S-1-5-7", // NT AUTHORITY\ANONYMOUS LOGON
			"S-1-5-8", // NT AUTHORITY\PROXY
			"S-1-5-9", // NT AUTHORITY\ENTERPRISE DOMAIN CONTROLLERS
			"S-1-5-10", // NT AUTHORITY\SELF
			"S-1-5-11", // NT AUTHORITY\Authenticated Users
			"S-1-5-12", // NT AUTHORITY\RESTRICTED
			"S-1-5-13", // NT AUTHORITY\TERMINAL SERVER USER
			"S-1-5-14", // NT AUTHORITY\REMOTE INTERACTIVE LOGON
			"", // Unknown
			"S-1-5-18", // NT AUTHORITY\SYSTEM
			"S-1-5-19", // NT AUTHORITY\LOCAL SERVICE
			"S-1-5-20", // NT AUTHORITY\NETWORK SERVICE
			"S-1-5-32", // BUILTIN\BUILTIN
			"S-1-5-32-544", // BUILTIN\Administrators
			"S-1-5-32-545", // BUILTIN\Users
			"S-1-5-32-546", // BUILTIN\Guests
			"S-1-5-32-547", // BUILTIN\Power Users
			"S-1-5-32-548", // BUILTIN\Account Operators
			"S-1-5-32-549", // BUILTIN\System Operators
			"S-1-5-32-550", // BUILTIN\Print Operators
			"S-1-5-32-551", // BUILTIN\Backup Operators
			"S-1-5-32-552", // BUILTIN\Replicator
			"S-1-5-32-554", // BUILTIN\PreWindows2000CompatibleAccess
			"S-1-5-32-555", // BUILTIN\Remote Desktop Users
			"S-1-5-32-556", // BUILTIN\Network Configuration Operators
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"", // Unknown
			"S-1-5-64-10", // NT AUTHORITY\NTLM Authentication
			"S-1-5-64-21", // NT AUTHORITY\Digest Authentication
			"S-1-5-64-14", // NT AUTHORITY\SChannel Authentication
			"S-1-5-15", // NT AUTHORITY\This Organization
			"S-1-5-1000", // NT AUTHORITY\Other Organization
			"S-1-5-32-557", // BUILTIN\Incoming Forest Trust Builders
			"S-1-5-32-558", // BUILTIN\Performance Monitor Users
			"S-1-5-32-559", // BUILTIN\Performance Log Users
			"S-1-5-32-560", // BUILTIN\Authorization Access
			"S-1-5-32-561", // BUILTIN\Terminal Server License Servers
			"S-1-5-32-562", // BUILTIN\Distributed COM Users
			"S-1-5-32-568", // BUILTIN\IIS_IUSRS
			"S-1-5-17", // NT AUTHORITY\IUSR
			"S-1-5-32-569", // BUILTIN\Cryptographic Operators
			"S-1-16-0", // Mandatory Label\Untrusted Mandatory Level
			"S-1-16-4096", // Mandatory Label\Low Mandatory Level
			"S-1-16-8192", // Mandatory Label\Medium Mandatory Level
			"S-1-16-12288", // Mandatory Label\High Mandatory Level
			"S-1-16-16384", // Mandatory Label\System Mandatory Level
			"S-1-5-33", // NT AUTHORITY\WRITE RESTRICTED
			"S-1-3-4", // OWNER RIGHTS
			"", // Unknown
			"", // Unknown
			"S-1-5-22", // NT AUTHORITY\ENTERPRISE READ-ONLY DOMAIN CONTROLLERS BETA
			"", // Unknown
			"S-1-5-32-573" // BUILTIN\Event Log Readers
		};

		/// <summary>
		/// Table of SDDL SID abbreviations
		/// </summary>
		/// <remarks>The table indicies correspond to <see cref="WELL_KNOWN_SID_TYPE"/> s</remarks>
		internal static readonly string[] WellKnownSIDAbbreviations = new string[]
		{
			"",
			"WD",
			"",
			"CO",
			"CG",
			"",
			"",
			"",
			"",
			"NU",
			"",
			"IU",
			"SU",
			"AN",
			"",
			"EC",
			"PS",
			"AU",
			"RC",
			"",
			"",
			"",
			"SY",
			"LS",
			"NS",
			"",
			"BA",
			"BU",
			"BG",
			"PU",
			"AO",
			"SO",
			"PO",
			"BO",
			"RE",
			"RU",
			"RD",
			"NO",
			"LA",
			"LG",
			"",
			"DA",
			"DU",
			"DG",
			"DC",
			"DD",
			"CA",
			"SA",
			"EA",
			"PA",
			"RS",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			"",
			""
		};
	}
}