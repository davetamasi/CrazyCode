using System;
using System.Diagnostics;
using Tamasi.Shared.Framework;

namespace Tamasi.Shared.WinFramework.Types
{
	/// <summary>
	/// The output string from !analyze, like 'AVRF_c015000f_xwtpdui.dll!CXWizardTypeDUI::_CXWizardTypeDUI'
	/// </summary>
	public sealed class FailureBucket
	{
		#region Fields and Constructors

		private readonly string failureBucketString = null;

		/// <summary>
		/// </summary>
		/// <param name="nodePath"></param>
		/// <param name="onFailure"></param>
		/// <param name="autoCorrect">
		/// If true, will trim and lowercase the depotFilePath argument, assuming all other checks pass
		/// </param>
		private FailureBucket
		(
			string failureBucketString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			this.failureBucketString = failureBucketString;
		}

		#endregion

		#region Properties

		//public ReadOnlyCollection<string> Nodes
		//{
		//	get
		//	{
		//	}
		//}

		#endregion

		#region Statics and Overrides

		public static FailureBucket Parse( string failureBucketString )
		{
			return new FailureBucket( failureBucketString );
		}

		public override string ToString()
		{
			return this.failureBucketString;
		}

		public static implicit operator string ( FailureBucket failureBucket )
		{
			return failureBucket.ToString();
		}

		public static implicit operator FailureBucket( string failureBucketString )
		{
			// TODO Pri 1 -- validate

			if( failureBucketString == null )
			{
				return null;
			}

			return new FailureBucket( failureBucketString );
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false.
			if( obj == null )
			{
				return false;
			}

			FailureBucket other = obj as FailureBucket;
			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return 0 == string.Compare( this.ToString(), other.ToString(), true );
		}

		public override Int32 GetHashCode()
		{
			return Hash.HashString32( this.failureBucketString );
		}

		public static Boolean IsValidFailureBucket
		(
			ref string failureBucketString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			// TODO Pri 3 Use a regex?

			Boolean isValid = true;

			if( string.IsNullOrEmpty( failureBucketString ) )
			{
				isValid = false;
			}
			else if( !CommonRegex.FailureBucketRegex.IsMatch( failureBucketString ) )
			{
				isValid = false;
			}

			if( !isValid && onFailure != ValidationFailureAction.Ignore )
			{
				string message = string.Format( "Invalid FailureBucket: {0}", failureBucketString );

				if( onFailure == ValidationFailureAction.Throw
					|| ( !Constants.IS_DEBUG_MODE && onFailure == ValidationFailureAction.Pivot ) )
				{
					throw new ScarabException( message );
				}
				else if( onFailure == ValidationFailureAction.Assert
					|| ( Constants.IS_DEBUG_MODE && onFailure == ValidationFailureAction.Pivot ) )
				{
					Debug.Fail( message );
				}
				else
				{
					Framework.Common.WriteLine( message );
				}
			}

			return isValid;
		}

		#endregion
	}
}