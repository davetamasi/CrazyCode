using System;
using System.Diagnostics;
using Tamasi.Shared.Framework;

namespace Tamasi.Shared.WinFramework.Types
{
	/// <summary>
	/// The canonical form of an Active Directory alias in uppercase
	/// (e.g., "BILLG" or "DTAMASI")
	/// </summary>
	public sealed class Alias
	{
		#region Fields and Constructors

		private readonly string aliasString = null;

		private Alias
		(
			string aliasString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			IsValidAlias( ref aliasString, onFailure );
			this.aliasString = aliasString;
		}

		#endregion

		#region Statics and Overrides

		public static Alias Parse( string aliasString )
		{
			return new Alias( aliasString );
		}

		public override string ToString()
		{
			return this.aliasString;
		}

		public static implicit operator string( Alias alias )
		{
			return alias.ToString();
		}

		public static implicit operator Alias( string aliasString )
		{
			if( aliasString == null )
			{
				return null;
			}

			return new Alias( aliasString );
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false.
			if( obj == null )
			{
				return false;
			}

			Alias other = obj as Alias;
			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return 0 == string.Compare( this.ToString(), other.ToString(), true );
		}

		public override Int32 GetHashCode()
		{
			return Hash.HashString32( this.aliasString );
		}

		public static Boolean IsValidAlias
		(
			ref string aliasString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			Boolean isValid = true;
			if( string.IsNullOrEmpty( aliasString ) )
			{
				isValid = false;
			}
			else
			{
				aliasString = aliasString.ToUpper();

				if( !CommonRegex.EmployeeAliasRegex.IsMatch( aliasString ) )
				{
					isValid = false;
				}
			}

			if( !isValid && onFailure != ValidationFailureAction.Ignore )
			{
				string message = string.Format( "Invalid Alias: {0}", aliasString );

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