namespace Tamasi.Shared.Framework
{
	/// <summary>
	/// Represents a three-state boolean (more useful than Boolean? in many cases)
	/// </summary>
	public enum TriBool
	{
		Null = -1,
		True = 0,
		False = 1
	}

	/// <summary>
	/// Action to take when a validator fails
	/// </summary>
	public enum ValidationFailureAction
	{
		/// <summary>
		/// Just return pass/fail to caller
		/// </summary>
		Ignore,

		/// <summary>
		/// Assert when validation fails (in debug mode only)
		/// </summary>
		Assert,

		/// <summary>
		/// Throw an exception when validation fails (e.g., use on tainted data)
		/// </summary>
		Throw,

		/// <summary>
		/// If DEBUG will assert, if RELEASE will throw (use for tainted data)
		/// </summary>
		Pivot
	}

	/// <summary>
	/// The kind of bug store -- test or production
	/// </summary>
	public enum BugStoreType
	{
		TEST,
		PRODUCTION,
	};

	public enum ExecutionType
	{
		Web = 1,
		Console = 2,
		Service = 3,
		Test = 4
	}

	/// <summary>
	/// In the context of a particualar target, what's the status of the run agaisnt it?
	/// </summary>
	public enum ExecStatus : byte
	{
		NOT_FOUND = 0,
		INCOMPLETE = 1,
		BLOCKED = 2,
		COMPLETE = 3,
	}

	/// <summary>
	/// The result of the TargetSelectionFlowchart, assuming a possible BuildLabel in the output variable
	/// </summary>
	public enum ExecAction : byte
	{
		NONE,
		BEGIN_EXEC,
		RESUME_EXEC,
		RERUN_EXEC
	}

	public enum StringMatchTypeEnum
	{
		StartsWith = 0,
		Contains = 1,
		EndsWith = 2,
		Is = 3
	}

	/// <summary>
	/// How to handle file copy/move collisions, where the target folder or file already exists
	/// </summary>
	public enum CollisionRemediation
	{
		/// <summary>
		/// Don't check in advance if target exists
		/// </summary>
		Passive,

		/// <summary>
		/// Don't transfer but return success
		/// </summary>
		Break,

		/// <summary>
		/// Only transfer what's not at the target
		/// </summary>
		FillIn,

		/// <summary>
		/// Delete target file/directory, then transfer
		/// </summary>
		Mirror,

		/// <summary>
		/// Transfer, attempting to overwrite whatever's there
		/// </summary>
		Overwrite,

		/// <summary>
		/// Don't transfer and throw an exception
		/// </summary>
		Throw
	}
}