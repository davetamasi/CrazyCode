using System;

namespace Tamasi.Shared.WinFramework.AccessControl
{
	[Flags]
	public enum AceRights
	{
		None = 0x00000000,
		GenericAll = 0x00000001,
		GenericRead = 0x00000002,
		GenericWrite = 0x00000004,
		GenericExecute = 0x00000008,
		StandardReadControl = 0x00000010,
		StandardDelete = 0x00000020,
		StandardWriteDAC = 0x00000040,
		StandardWriteOwner = 0x00000080,
		DirectoryReadProperty = 0x00000100,
		DirectoryWriteProperty = 0x00000200,
		DirectoryCreateChild = 0x00000400,
		DirectoryDeleteChild = 0x00000800,
		DirectoryListChildren = 0x00001000,
		DirectorySelfWrite = 0x00002000,
		DirectoryListObject = 0x00004000,
		DirectoryDeleteTree = 0x00008000,
		DirectoryControlAccess = 0x00010000,
		FileAll = 0x00020000,
		FileRead = 0x00040000,
		FileWrite = 0x00080000,
		FileExecute = 0x00100000,
		KeyAll = 0x00200000,
		KeyRead = 0x00400000,
		KeyWrite = 0x00800000,
		KeyExecute = 0x01000000
	}

	public enum AceType
	{
		AccessAllowed = 0,
		AccessDenied,
		ObjectAccessAllowed,
		ObjectAccessDenied,
		Audit,
		Alarm,
		ObjectAudit,
		ObjectAlarm
	}

	[Flags]
	public enum AceFlags
	{
		None = 0x0000,
		ContainerInherit = 0x0001,
		ObjectInherit = 0x0002,
		NoPropogate = 0x0004,
		InheritOnly = 0x0008,
		Inherited = 0x0010,
		AuditSuccess = 0x0020,
		AuditFailure = 0x0040
	}

	/// <summary>
	/// Access Control List Flags
	/// </summary>
	[Flags]
	public enum AclFlags
	{
		None = 0x00,
		Protected = 0x01,
		MustInherit = 0x02,
		Inherited = 0x04
	}
}