using System;

namespace Tamasi.Shared.WinFramework.Lsa
{
	internal enum SidNameUse : Int32
	{
		User = 1,
		Group = 2,
		Domain = 3,
		Alias = 4,
		KnownGroup = 5,
		DeletedAccount = 6,
		Invalid = 7,
		Unknown = 8,
		Computer = 9
	}

	internal enum Access : Int32
	{
		POLICY_READ = 0x20006,
		POLICY_ALL_ACCESS = 0x00F0FFF,
		POLICY_EXECUTE = 0X20801,
		POLICY_WRITE = 0X207F8
	}

	public enum WindowsPrivilege
	{
		SeAssignPrimaryTokenPrivilege,
		SeAuditPrivilege,
		SeBackupPrivilege,
		SeBatchLogonRight,
		SeChangeNotifyPrivilege,
		SeCreateGlobalPrivilege,
		SeCreatePagefilePrivilege,
		SeCreatePermanentPrivilege,
		SeCreateSymbolicLinkPrivilege,
		SeCreateTokenPrivilege,
		SeDebugPrivilege,
		SeDenyBatchLogonRight,
		SeDenyInteractiveLogonRight,
		SeDenyNetworkLogonRight,
		SeDenyRemoteInteractiveLogonRight,
		SeDenyServiceLogonRight,
		SeEnableDelegationPrivilege,
		SeImpersonatePrivilege,
		SeIncreaseBasePriorityPrivilege,
		SeIncreaseQuotaPrivilege,
		SeIncreaseWorkingSetPrivilege,
		SeInteractiveLogonRight,
		SeLoadDriverPrivilege,
		SeLockMemoryPrivilege,
		SeMachineAccountPrivilege,
		SeManageVolumePrivilege,
		SeNetworkLogonRight,
		SeProfileSingleProcessPrivilege,
		SeRelabelPrivilege,
		SeRemoteInteractiveLogonRight,
		SeRemoteShutdownPrivilege,
		SeRestorePrivilege,
		SeSecurityPrivilege,
		SeServiceLogonRight,
		SeShutdownPrivilege,
		SeSyncAgentPrivilege,
		SeSystemEnvironmentPrivilege,
		SeSystemProfilePrivilege,
		SeSystemtimePrivilege,
		SeTakeOwnershipPrivilege,
		SeTcbPrivilege,
		SeTimeZonePrivilege,
		SeTrustedCredManAccessPrivilege,
		SeUndockPrivilege,
		SeUnsolicitedInputPrivilege
	}
}