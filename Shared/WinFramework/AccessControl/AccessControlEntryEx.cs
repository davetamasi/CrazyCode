using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Tamasi.Shared.WinFramework.AccessControl
{
	public sealed class AccessControlEntryEx
	{
		#region Fields and Constructors

		private static readonly string[] aceTypeStrings = new string[] { "A", "D", "OA", "OD", "AU", "AL", "OU", "OL" };
		private static readonly string[] aceFlagStrings = new string[] { "CI", "OI", "NP", "IO", "ID", "SA", "FA" };

		private static readonly string[] rightsStrings = new string[]
		{
			"GA",
			"GR",
			"GW",
			"GX",
			"RC",
			"SD",
			"WD",
			"WO",
			"RP",
			"WP",
			"CC",
			"DC",
			"LC",
			"SW",
			"LO",
			"DT",
			"CR",
			"FA",
			"FR",
			"FW",
			"FX",
			"KA",
			"KR",
			"KW",
			"KX"
		};

		private const string cAceExpr = @"^(?'ace_type'[A-Z]+)?;(?'ace_flags'([A-Z]{2})+)?;(?'rights'([A-Z]{2})+|0x[0-9A-Fa-f]+)?;(?'object_guid'[0-9A-Fa-f\-]+)?;(?'inherit_object_guid'[0-9A-Fa-f\-]+)?;(?'account_sid'[A-Z]+?|S(-[0-9]+)+)?$";

		private readonly AceType aceType = AceType.AccessAllowed;
		private readonly AceFlags flags = AceFlags.None;
		private readonly AceRights rights = AceRights.None;
		private readonly Guid objectGuid = Guid.Empty;
		private readonly Guid inheritObjectGuid = Guid.Empty;
		private readonly SecurityIdentity accountSID;

		/// <summary>
		/// Creates a Access Control Entry with account SID
		/// </summary>
		/// <param name="account">Account SID</param>
		public AccessControlEntryEx( SecurityIdentity account )
		{
			this.accountSID = account;
		}

		/// <summary>
		/// Creates a Access Control Entry of type AccessAllowed with account SID and Rights
		/// </summary>
		/// <param name="account">Account SID</param>
		/// <param name="rights">Rights</param>
		public AccessControlEntryEx( SecurityIdentity account, AceRights rights )
		{
			this.accountSID = account;
			this.rights = rights;
		}

		/// <summary>
		/// Creates a Access Control Entry of type AccessAllowed with account SID, Type and Rights
		/// </summary>
		/// <param name="account">Account SID</param>
		/// <param name="aceType">Type of Access Control</param>
		/// <param name="rights">Rights</param>
		public AccessControlEntryEx( SecurityIdentity account, AceType aceType, AceRights rights )
		{
			this.accountSID = account;
			this.aceType = aceType;
			this.rights = rights;
		}

		/// <summary>
		/// Creates a deep copy of an existing Access Control Entry
		/// </summary>
		/// <param name="original">Original AccessControlEntry</param>
		public AccessControlEntryEx( AccessControlEntryEx original )
		{
			this.accountSID = original.accountSID;
			this.aceType = original.aceType;
			this.flags = original.flags;
			this.inheritObjectGuid = original.inheritObjectGuid;
			this.objectGuid = original.objectGuid;
			this.rights = original.rights;
		}

		/// <summary>
		/// Creates a Access Control Entry from a ACE string
		/// </summary>
		/// <param name="aceString">ACE string</param>
		public AccessControlEntryEx( string aceString )
		{
			Regex aceRegex = new Regex( cAceExpr, RegexOptions.IgnoreCase );

			Match aceMatch = aceRegex.Match( aceString );
			if( !aceMatch.Success )
			{
				throw new FormatException( "Invalid ACE String Format" );
			}

			if( aceMatch.Groups[ "ace_type" ] != null && aceMatch.Groups[ "ace_type" ].Success && !String.IsNullOrEmpty( aceMatch.Groups[ "ace_type" ].Value ) )
			{
				Int32 aceTypeValue = Array.IndexOf<string>( AccessControlEntryEx.aceTypeStrings, aceMatch.Groups[ "ace_type" ].Value.ToUpper() );

				if( aceTypeValue == -1 ) throw new FormatException( "Invalid ACE String Format" );

				this.aceType = ( AceType )aceTypeValue;
			}
			else
			{
				throw new FormatException( "Invalid ACE String Format" );
			}

			if( aceMatch.Groups[ "ace_flags" ] != null && aceMatch.Groups[ "ace_flags" ].Success && !String.IsNullOrEmpty( aceMatch.Groups[ "ace_flags" ].Value ) )
			{
				string aceFlagsValue = aceMatch.Groups[ "ace_flags" ].Value.ToUpper();
				for( Int32 i = 0; i < aceFlagsValue.Length - 1; i += 2 )
				{
					Int32 flagValue = Array.IndexOf<string>( AccessControlEntryEx.aceFlagStrings, aceFlagsValue.Substring( i, 2 ) );

					if( flagValue == -1 ) throw new FormatException( "Invalid ACE String Format" );

					this.flags = this.flags | ( ( AceFlags )( Int32 )Math.Pow( 2.0d, flagValue ) );
				}
			}

			if( aceMatch.Groups[ "rights" ] != null && aceMatch.Groups[ "rights" ].Success && !String.IsNullOrEmpty( aceMatch.Groups[ "rights" ].Value ) )
			{
				string rightsValue = aceMatch.Groups[ "rights" ].Value.ToUpper();
				for( Int32 i = 0; i < rightsValue.Length - 1; i += 2 )
				{
					Int32 rightValue = Array.IndexOf<string>( AccessControlEntryEx.rightsStrings, rightsValue.Substring( i, 2 ) );

					if( rightValue == -1 )
					{
						throw new FormatException( "Invalid ACE String Format" );
					}

					this.rights = this.rights | ( AceRights )( Int32 )Math.Pow( 2.0d, rightValue );
				}
			}

			if( aceMatch.Groups[ "object_guid" ] != null && aceMatch.Groups[ "object_guid" ].Success && !String.IsNullOrEmpty( aceMatch.Groups[ "object_guid" ].Value ) )
			{
				this.objectGuid = new Guid( aceMatch.Groups[ "object_guid" ].Value );
			}

			if( aceMatch.Groups[ "inherit_object_guid" ] != null && aceMatch.Groups[ "inherit_object_guid" ].Success && !String.IsNullOrEmpty( aceMatch.Groups[ "inherit_object_guid" ].Value ) )
			{
				this.inheritObjectGuid = new Guid( aceMatch.Groups[ "inherit_object_guid" ].Value );
			}

			if( aceMatch.Groups[ "account_sid" ] != null && aceMatch.Groups[ "account_sid" ].Success && !String.IsNullOrEmpty( aceMatch.Groups[ "account_sid" ].Value ) )
			{
				this.accountSID = SecurityIdentity.SecurityIdentityFromSIDorAbbreviation( aceMatch.Groups[ "account_sid" ].Value.ToUpper() );
			}
			else
			{
				throw new FormatException( "Invalid ACE String Format" );
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Returns IEnumerable of rights
		/// </summary>
		/// <returns>IEnumerable of rights</returns>
		public IEnumerable<AceRights> GetRightsEnumerator()
		{
			Int32 current = ( Int32 )AceRights.GenericAll;

			for( Int32 col = ( Int32 )this.rights; col != 0; col = col >> 1, current = current << 1 )
			{
				while( col != 0 && ( col & 1 ) != 1 )
				{
					col = col >> 1;
					current = current << 1;
				}

				if( ( col & 1 ) == 1 )
				{
					yield return ( AceRights )current;
				}
			}
		}

		/// <summary>
		/// Renders the Access Control Entry as an SDDL ACE string
		/// </summary>
		/// <returns>An SDDL ACE string.</returns>
		public override String ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat( "{0};", AccessControlEntryEx.aceTypeStrings[ ( Int32 )this.aceType ] );

			for( Int32 flag = 0x01; flag <= ( Int32 )AceFlags.AuditFailure; flag = flag << 1 )
			{
				if( ( flag & ( Int32 )this.flags ) == flag ) sb.Append( AccessControlEntryEx.aceFlagStrings[ ( Int32 )Math.Log( flag, 2.0d ) ] );
			}

			sb.Append( ';' );

			foreach( var right in this.GetRightsEnumerator() )
			{
				sb.Append( AccessControlEntryEx.rightsStrings[ ( Int32 )Math.Log( ( Int32 )right, 2.0d ) ] );
			}

			sb.Append( ';' );

			sb.AppendFormat( "{0};", this.objectGuid != Guid.Empty ? this.objectGuid.ToString() : "" );

			sb.AppendFormat( "{0};", this.inheritObjectGuid != Guid.Empty ? this.inheritObjectGuid.ToString() : "" );

			if( this.accountSID != null ) sb.Append( this.accountSID.ToString() );

			return sb.ToString();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or Sets the Access Control Entry Type
		/// </summary>
		public AceType AceType
		{
			get { return this.aceType; }
			//set { this.aceType = value; }
		}

		/// <summary>
		/// Gets or Sets the Access Control Entry Flags
		/// </summary>
		public AceFlags Flags
		{
			get { return this.flags; }
			//set { this.flags = value; }
		}

		/// <summary>
		/// Gets or Sets the Access Control Entry Rights
		/// </summary>
		/// <remarks>
		/// This is a binary flag value, and can be more easily accessed via the Access Control
		/// Entry collection methods.
		/// </remarks>
		public AceRights Rights
		{
			get { return this.rights; }
			//set { this.rights = value; }
		}

		/// <summary>
		/// Gets or Sets the Object Guid
		/// </summary>
		public Guid ObjectGuid
		{
			get { return this.objectGuid; }
			//set { this.objectGuid = value; }
		}

		/// <summary>
		/// Gets or Sets the Inherit Object Guid
		/// </summary>
		public Guid InheritObjectGuid
		{
			get { return this.inheritObjectGuid; }
			//set { this.inheritObjectGuid = value; }
		}

		/// <summary>
		/// Gets or Sets the Account SID
		/// </summary>
		public SecurityIdentity AccountSID
		{
			get { return this.accountSID; }
			//set { this.accountSID = value; }
		}

		#endregion
	}
}