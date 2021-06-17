using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tamasi.Shared.Framework;

namespace Tamasi.Shared.WinFramework.Types
{
	/// <summary>
	/// A file path in canonical form:  depot\dir\dir\file.c
	/// </summary>
	public sealed class CanonicalFilePath
	{
		#region Fields and Constructors

		private readonly String cfpString = null;
		private readonly Match match = null;

		/// <summary>
		/// </summary>
		/// <param name="nodePath"></param>
		/// <param name="onFailure"></param>
		private CanonicalFilePath
		(
			String cfpString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			IsValid( ref cfpString, onFailure );
			this.cfpString = cfpString;

			this.match = CommonRegex.CanonicalFilePathRegex.Match( this.cfpString );
			Debug.Assert( this.match.Success );
		}

		#endregion

		#region Properties

		public String DepotName
		{
			get { return this.match.Groups[ 2 ].Value.ToUpper(); }
		}

		public String FileName
		{
			get { return this.match.Groups[ 3 ].Value.ToLower(); }
		}

		/// <summary>
		/// The file extension in all upper case with no dot (e.g., "CPP")
		/// </summary>
		public String FileExtension
		{
			get { return Path.GetExtension( this.FileName ).Replace( ".", String.Empty ).ToUpper(); }
		}

		/// <summary>
		/// Returns the directory path with the terminal slash (e.g., "depot\dir1\dir2\dir3\")
		/// </summary>
		public String DirectoryPath
		{
			get { return this.match.Groups[ 1 ].Value.ToLower(); }
		}

		/// <summary>
		/// Returns the directory path without the terminal slash (e.g., "depot\dir1\dir2\dir3")
		/// </summary>
		public String DirectoryPathTrimmed
		{
			get { return this.DirectoryPath.TrimEnd( this.DirectoryPath[ this.DirectoryPath.Length - 1 ] ); }
		}

		public ReadOnlyCollection<String> DirectoryNodes
		{
			get
			{
				return this.DirectoryPath.Split
				(
					new char[] { '\\' },
					StringSplitOptions.RemoveEmptyEntries
				).ToList().AsReadOnly();
			}
		}

		#endregion

		#region Statics and Overrides

		public static CanonicalFilePath Parse( String canonicalFilePathString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			return new CanonicalFilePath( canonicalFilePathString, onFailure );
		}

		public override String ToString()
		{
			return this.cfpString;
		}

		public static implicit operator String( CanonicalFilePath cfp )
		{
			return cfp.ToString();
		}

		public static implicit operator CanonicalFilePath( String cfpString )
		{
			// TODO Pri 1 -- validate

			if( !CommonRegex.CanonicalFilePathRegex.IsMatch( cfpString ) )
			{
				return null;
			}

			return new CanonicalFilePath( cfpString );
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false.
			if( obj == null )
			{
				return false;
			}

			CanonicalFilePath other = obj as CanonicalFilePath;
			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return 0 == String.Compare( this.ToString(), other.ToString(), true );
		}

		public override Int32 GetHashCode()
		{
			return Hash.HashString32( this.cfpString );
		}

		public static Boolean IsValid
		(
			ref String cfpString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			Boolean isValid = true;

			if( String.IsNullOrEmpty( cfpString ) )
			{
				isValid = false;
			}
			else if( !CommonRegex.CanonicalFilePathRegex.IsMatch( cfpString ) )
			{
				isValid = false;
			}

			if( !isValid && onFailure != ValidationFailureAction.Ignore )
			{
				String message = String.Format( "Invalid CanonicalFilePath: {0}", cfpString );

				if( onFailure == ValidationFailureAction.Throw
					|| ( !Constants.IS_DEBUG_MODE && onFailure == ValidationFailureAction.Pivot ) )
				{
					throw new ArgumentException( message, nameof( cfpString ) );
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