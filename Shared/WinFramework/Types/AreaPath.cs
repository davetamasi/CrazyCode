using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Tamasi.Shared.Framework;

namespace Tamasi.Shared.WinFramework.Types
{
	/// <summary>
	/// The canonical form of a CompCentral/TFS area path -- \TeamProject\Node1\Node2\Node3\
	/// </summary>
	public sealed class AreaPath
	{
		#region Fields and Constructors

		private readonly string areaPathString = null;

		/// <summary>
		/// </summary>
		/// <param name="nodePath"></param>
		/// <param name="onFailure"></param>
		/// <param name="autoCorrect">
		/// If true, will trim and lowercase the depotFilePath argument, assuming all other checks pass
		/// </param>
		private AreaPath
		(
			string areaPathString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			IsValidAreaPath( ref areaPathString, onFailure );
			this.areaPathString = areaPathString;
		}

		#endregion

		#region Properties

		public ReadOnlyCollection<string> Nodes
		{
			get
			{
				return this.areaPathString.Split( new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries ).ToList().AsReadOnly();
			}
		}

		#endregion

		#region Statics and Overrides

		/// <summary>
		/// Parses a string and, if it's a valid area path, returns an AreaPath object,
		/// NULL otherwise.  The default is to ignore null or invalid input, but this can
		/// be set via the ValidationFailureAction at onFailure.
		/// </summary>
		public static AreaPath Parse
		(
			String areaPathString,
			ValidationFailureAction onFailure = ValidationFailureAction.Ignore )
		{
			return new AreaPath( areaPathString, onFailure );
		}

		public override string ToString()
		{
			return this.areaPathString;
		}

		public static implicit operator string ( AreaPath areaPath )
		{
			return areaPath.ToString();
		}

		public static implicit operator AreaPath( string areaPathString )
		{
			// TODO Pri 1 -- validate

			if( areaPathString == null )
			{
				return null;
			}

			return new AreaPath( areaPathString );
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false.
			if( obj == null )
			{
				return false;
			}

			AreaPath other = obj as AreaPath;
			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return 0 == string.Compare( this.ToString(), other.ToString(), true );
		}

		public override Int32 GetHashCode()
		{
			return Hash.HashString32( this.areaPathString );
		}

		public static Boolean IsValidAreaPath
		(
			ref string areaPathString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			// TODO Pri 3 Use a regex?

			Boolean isValid = true;

			if( string.IsNullOrEmpty( areaPathString ) )
			{
				isValid = false;
			}
			else if( !CommonRegex.AreaPathRegex.IsMatch( areaPathString ) )
			{
				isValid = false;
			}

			if( !isValid && onFailure != ValidationFailureAction.Ignore )
			{
				string message = string.Format( "Invalid AreaPath: {0}", areaPathString );

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