using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Security.Principal;

using Tamasi.Shared.Framework;
using Tamasi.Shared.Framework.FileSystemUtilities;

namespace Tamasi.Shared.WinFramework.Types
{
	public sealed class SddlString
	{
		#region Fields and Constructors

		private readonly string sddlString = null;

		/// <summary>
		///
		/// </summary>
		private SddlString
		(
			string sddlString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			IsValidSddlString( ref sddlString, onFailure );
			this.sddlString = sddlString;
		}

		#endregion

		#region Public Statics

		public static string Parse( string sddlString )
		{
			return Parse( sddlString, "\r\n", "\t" );
		}

		public static string Parse( string sddlString, string separator, string separator2 )
		{
			string retval = "";
			if( ACE_Types == null )
			{
				Initialize();
			}
			Int32 startindex = 0;
			Int32 nextindex = 0;
			Int32 first = 0;
			string section;

			while( true )
			{
				first = sddlString.IndexOf( ':', nextindex ) - 1;
				startindex = nextindex;
				if( first < 0 )
				{
					break;
				}
				if( first != 0 )
				{
					section = sddlString.Substring( startindex - 2, first - startindex + 2 );
					retval += doParse( section, separator, separator2 );
				}
				nextindex = first + 2;
			}
			section = sddlString.Substring( startindex - 2 );
			retval += doParse( section, separator, separator2 );
			return retval;
		}

		public static Boolean IsValidSddlString
		(
			ref string sddlString,
			ValidationFailureAction onFailure = ValidationFailureAction.Pivot )
		{
			Boolean isValid = false;

			if( !string.IsNullOrEmpty( sddlString ) && CommonRegex.SddlStringRegex.IsMatch( sddlString ) )
			{
				isValid = true;
			}

			if( !isValid && onFailure != ValidationFailureAction.Ignore )
			{
				string message = string.Format( "Invalid SddlString: {0}", sddlString );

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

		#region Private Statics

		static private Dictionary<string, string>? ACE_Types = null;
		static private Dictionary<string, string>? ACE_Flags = null;
		static private Dictionary<string, string>? Permissions = null;
		static private Dictionary<string, string>? Trustee = null;

		private static void Initialize()
		{
			ACE_Types = new Dictionary<string, string>();
			ACE_Flags = new Dictionary<string, string>();
			Permissions = new Dictionary<string, string>();
			Trustee = new Dictionary<string, string>();

			#region Add ACE_Types

			ACE_Types.Add( "A", "Access Allowed" );
			ACE_Types.Add( "D", "Access Denied" );
			ACE_Types.Add( "OA", "Object Access Allowed" );
			ACE_Types.Add( "OD", "Object Access Denied" );
			ACE_Types.Add( "AU", "System Audit" );
			ACE_Types.Add( "AL", "System Alarm" );
			ACE_Types.Add( "OU", "Object System Audit" );
			ACE_Types.Add( "OL", "Object System Alarm" );

			#endregion

			#region Add ACE_Flags

			ACE_Flags.Add( "CI", "Container Inherit" );
			ACE_Flags.Add( "OI", "Object Inherit" );
			ACE_Flags.Add( "NP", "No Propogate" );
			ACE_Flags.Add( "IO", "Inheritance Only" );
			ACE_Flags.Add( "ID", "Inherited" );
			ACE_Flags.Add( "SA", "Successful Access Audit" );
			ACE_Flags.Add( "FA", "Failed Access Audit" );

			#endregion

			#region Add Permissions

			#region Generic Access Rights

			Permissions.Add( "GA", "Generic All" );
			Permissions.Add( "GR", "Generic Read" );
			Permissions.Add( "GW", "Generic Write" );
			Permissions.Add( "GX", "Generic Execute" );

			#endregion

			#region Directory Access Rights

			Permissions.Add( "RC", "Read Permissions" );
			Permissions.Add( "SD", "Delete" );
			Permissions.Add( "WD", "Modify Permissions" );
			Permissions.Add( "WO", "Modify Owner" );
			Permissions.Add( "RP", "Read All Properties" );
			Permissions.Add( "WP", "Write All Properties" );
			Permissions.Add( "CC", "Create All Child Objects" );
			Permissions.Add( "DC", "Delete All Child Objects" );
			Permissions.Add( "LC", "List Contents" );
			Permissions.Add( "SW", "All Validated Writes" );
			Permissions.Add( "LO", "List Object" );
			Permissions.Add( "DT", "Delete Subtree" );
			Permissions.Add( "CR", "All Extended Rights" );

			#endregion

			#region File Access Rights

			Permissions.Add( "FA", "File All Access" );
			Permissions.Add( "FR", "File Generic Read" );
			Permissions.Add( "FW", "File Generic Write" );
			Permissions.Add( "FX", "File Generic Execute" );

			#endregion

			#region Registry Key Access Rights

			Permissions.Add( "KA", "Key All Access" );
			Permissions.Add( "KR", "Key Read" );
			Permissions.Add( "KW", "Key Write" );
			Permissions.Add( "KX", "Key Execute" );

			#endregion

			#endregion

			#region Add Trustees

			Trustee.Add( "AO", "Account Operators" );
			Trustee.Add( "RU", "Alias to allow previous Windows 2000" );
			Trustee.Add( "AN", "Anonymous Logon" );
			Trustee.Add( "AU", "Authenticated Users" );
			Trustee.Add( "BA", "Built-in Administrators" );
			Trustee.Add( "BG", "Built in Guests" );
			Trustee.Add( "BO", "Backup Operators" );
			Trustee.Add( "BU", "Built-in Users" );
			Trustee.Add( "CA", "Certificate Server Administrators" );
			Trustee.Add( "CG", "Creator Group" );
			Trustee.Add( "CO", "Creator Owner" );
			Trustee.Add( "DA", "Domain Administrators" );
			Trustee.Add( "DC", "Domain Computers" );
			Trustee.Add( "DD", "Domain Controllers" );
			Trustee.Add( "DG", "Domain Guests" );
			Trustee.Add( "DU", "Domain Users" );
			Trustee.Add( "EA", "Enterprise Administrators" );
			Trustee.Add( "ED", "Enterprise Domain Controllers" );
			Trustee.Add( "WD", "Everyone" );
			Trustee.Add( "PA", "Group Policy Administrators" );
			Trustee.Add( "IU", "Interactively logged-on user" );
			Trustee.Add( "LA", "Local Administrator" );
			Trustee.Add( "LG", "Local Guest" );
			Trustee.Add( "LS", "Local Service Account" );
			Trustee.Add( "SY", "Local System" );
			Trustee.Add( "NU", "Network Logon User" );
			Trustee.Add( "NO", "Network Configuration Operators" );
			Trustee.Add( "NS", "Network Service Account" );
			Trustee.Add( "PO", "Printer Operators" );
			Trustee.Add( "PS", "Self" );
			Trustee.Add( "PU", "Power Users" );
			Trustee.Add( "RS", "RAS Servers group" );
			Trustee.Add( "RD", "Terminal Server Users" );
			Trustee.Add( "RE", "Replicator" );
			Trustee.Add( "RC", "Restricted Code" );
			Trustee.Add( "SA", "Schema Administrators" );
			Trustee.Add( "SO", "Server Operators" );
			Trustee.Add( "SU", "Service Logon User" );

			#endregion
		}

		private static string doParse( string subSDDL, string seperator, string seperator2 )
		{
			Debug.Assert( !string.IsNullOrEmpty( subSDDL ) );
			Debug.Assert( !string.IsNullOrEmpty( seperator ) );
			Debug.Assert( !string.IsNullOrEmpty( seperator2 ) );

			StringBuilder sb = new StringBuilder();
			char sddlType = subSDDL.ToCharArray()[ 0 ];

			switch( sddlType )
			{
				case 'O':
					string owner = subSDDL.Substring( 2 );
					if( Trustee.Keys.Contains( owner ) )
					{
						sb.AppendFormat( "Owner: {0}{1}", Trustee[ owner ], seperator );
					}

					break;
				case 'G':
					string group = subSDDL.Substring( 2 );
					if( Trustee.Keys.Contains( group ) )
					{
						sb.AppendFormat( "Group: {0}{1}", Trustee[ group ], seperator );
					}

					break;
				case 'D':
				case 'S':

					if( sddlType == 'D' )
					{
						sb.AppendFormat( "DACL{0}", seperator );
					}
					else
					{
						sb.AppendFormat( "SACL{0}", seperator );
					}

					string[] sections = subSDDL.Split( '(' );

					for( Int32 count = 1; count < sections.Length; count++ )
					{
						sb.AppendLine( "# " + count.ToString() + " of " + ( sections.Length - 1 ).ToString() + seperator );
						string[] parts = sections[ count ].TrimEnd( ')' ).Split( ';' );
						sb.AppendFormat( "" );
						if( ACE_Types.Keys.Contains( parts[ 0 ] ) )
						{
							sb.AppendLine( seperator2 + "Type: " + ACE_Types[ parts[ 0 ] ] + seperator );
						}
						if( ACE_Flags.Keys.Contains( parts[ 1 ] ) )
						{
							sb.AppendLine( seperator2 + "Inheritance: " + ACE_Flags[ parts[ 1 ] ] + seperator );
						}
						for( Int32 count2 = 0; count2 < parts[ 2 ].Length; count2 += 2 )
						{
							string perm = parts[ 2 ].Substring( count2, 2 );
							if( Permissions.Keys.Contains( perm ) )
							{
								if( count2 == 0 )
								{
									sb.AppendLine( seperator2 + "Permissions: " + Permissions[ perm ] );
								}
								else
								{
									sb.AppendLine( "|" + Permissions[ perm ] );
								}
							}
						}
						sb.Append( seperator );
						if( Trustee.Keys.Contains( parts[ 5 ] ) )
						{
							sb.AppendLine( seperator2 + "Trustee: " + Trustee[ parts[ 5 ] ] + seperator );
						}
						else
						{
							try
							{
								SecurityIdentifier sid = new SecurityIdentifier( parts[ 5 ] );
								sb.AppendLine( seperator2 + "Trustee: " + sid.Translate( typeof( System.Security.Principal.NTAccount ) ).ToString() + seperator );
							}
							catch( Exception )
							{
								sb.AppendLine( seperator2 + "Trustee: " + parts[ 5 ] + seperator );
							}
						}
					}
					break;
				default:
					throw new ArgumentException( "Invalid SDDL Type" );
					break;
			}

			return sb.ToString();
		}

		#endregion
	}
}