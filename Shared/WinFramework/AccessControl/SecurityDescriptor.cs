using System;
using System.Text;
using System.Text.RegularExpressions;

using fr = Tamasi.Shared.Framework;
using Tamasi.Shared.WinFramework;
using Tamasi.Shared.WinFramework.Types;

namespace Tamasi.Shared.WinFramework.AccessControl
{
	/// <summary>
	/// Security Descriptor
	/// </summary>
	/// <remarks>
	/// The Security Descriptor is the top level of the Access Control API. It represents all the
	/// Access Control data that is associated with the secured object.
	/// </remarks>
	public class SecurityDescriptor
	{
		#region Fields and Constructors

		private SddlString sddlString;
		private SecurityIdentity ownerSid;
		private SecurityIdentity groupSid;
		private AccessControlListEx dacl;
		private AccessControlListEx sacl;

		/// <summary>
		/// Creates a blank Security Descriptor
		/// </summary>
		public SecurityDescriptor()
		{
		}

		/// <summary>
		/// Creates a Security Descriptor from an SDDL string
		/// </summary>
		/// <param name="sddlString">The SDDL string that represents the Security Descriptor</param>
		/// <exception cref="System.FormatException">Invalid SDDL String Format</exception>
		public SecurityDescriptor( SddlString sddlString )
		{
			Match m = CommonRegex.SddlStringRegex.Match( sddlString.ToString() );

			if( !m.Success )
			{
				throw new FormatException( "Invalid SDDL String Format" );
			}

			this.sddlString = sddlString;

			if( m.Groups[ "owner" ] != null && m.Groups[ "owner" ].Success && !String.IsNullOrEmpty( m.Groups[ "owner" ].Value ) )
			{
				this.Owner = SecurityIdentity.SecurityIdentityFromSIDorAbbreviation( m.Groups[ "owner" ].Value );
			}

			if( m.Groups[ "group" ] != null && m.Groups[ "group" ].Success && !String.IsNullOrEmpty( m.Groups[ "group" ].Value ) )
			{
				this.Group = SecurityIdentity.SecurityIdentityFromSIDorAbbreviation( m.Groups[ "group" ].Value );
			}

			if( m.Groups[ "dacl" ] != null && m.Groups[ "dacl" ].Success && !String.IsNullOrEmpty( m.Groups[ "dacl" ].Value ) )
			{
				this.DACL = new AccessControlListEx( m.Groups[ "dacl" ].Value );
			}

			if( m.Groups[ "sacl" ] != null && m.Groups[ "sacl" ].Success && !String.IsNullOrEmpty( m.Groups[ "sacl" ].Value ) )
			{
				this.SACL = new AccessControlListEx( m.Groups[ "sacl" ].Value );
			}
		}

		#endregion

		#region Overrides

		/// <summary>
		/// Renders the Security Descriptor as an SDDL string
		/// </summary>
		/// <remarks>
		/// For more info on SDDL see <a
		/// href="http://msdn.microsoft.com/en-us/library/aa379570(VS.85).aspx">MSDN: Security
		/// Descriptor String Format.</a>
		/// </remarks>
		/// <returns>An SDDL string</returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if( this.ownerSid != null )
			{
				sb.AppendFormat( "O:{0}", this.ownerSid.ToString() );
			}

			if( this.groupSid != null )
			{
				sb.AppendFormat( "G:{0}", this.groupSid.ToString() );
			}

			if( this.dacl != null )
			{
				sb.AppendFormat( "D:{0}", this.dacl.ToString() );
			}

			if( this.sacl != null )
			{
				sb.AppendFormat( "S:{0}", this.sacl.ToString() );
			}

			return sb.ToString();
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or Sets the Owner
		/// </summary>
		public SecurityIdentity Owner
		{
			get { return this.ownerSid; }
			set { this.ownerSid = value; }
		}

		/// <summary>
		/// Gets or Sets the Group
		/// </summary>
		/// <remarks>
		/// Security Descriptor Groups are present for Posix compatibility reasons and are usually ignored.
		/// </remarks>
		public SecurityIdentity Group
		{
			get { return this.groupSid; }
			set { this.groupSid = value; }
		}

		/// <summary>
		/// Gets or Sets the DACL
		/// </summary>
		/// <remarks>
		/// The DACL (Discretionary Access Control List) is the Access Control List that grants or
		/// denies various types of access for different users and groups.
		/// </remarks>
		public AccessControlListEx DACL
		{
			get { return this.dacl; }
			set { this.dacl = value; }
		}

		/// <summary>
		/// Gets or Sets the SACL
		/// </summary>
		/// <remarks>
		/// The SACL (System Access Control List) is the Access Control List that specifies what
		/// actions should be auditted
		/// </remarks>
		public AccessControlListEx SACL
		{
			get { return this.sacl; }
			set { this.sacl = value; }
		}

		#endregion
	}
}