using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using fr = Tamasi.Shared.Framework;

namespace Tamasi.Shared.WinFramework.Types
{
	/// <summary>
	/// Represents the four- (sometimes five)-part naming scheme for Windows builds
	/// (irrespective of SKU, Chunk, Architecture, etc.)
	/// </summary>
	[TypeConverter( typeof( BuildLabelConverter ) )]
	public sealed class BuildLabel
	{
		#region Fields and Constructors

		// http: //windowssites/sites/winbuilddocs/Wiki%20Pages/BNS_Using%20the%20Build%20Notification%20Service.aspx

		private readonly string branchName;
		private readonly Int16 buildNumber;
		private readonly Int16 buildQfe;
		private readonly string buildRevision;
		private readonly Int16 dashNumber;

		/// <summary>
		///
		/// </summary>
		/// <param name="branchName"></param>
		/// <param name="buildNumber">Part of the identification of the build. Ex: In the build name “7100.0.winmain_win7rc.090421-1700”, the Version would be 7100.</param>
		/// <param name="buildQfe">Part of the identification of the build. Ex: In the build name “7100.0.winmain_win7rc. 090421-1700”, the Qfe would be 0.</param>
		/// <param name="buildRevision"></param>
		/// <param name="dashNumber">Part of the identification of the build.  This is used when a build is only recreating the language-specific parts of the build, which typically occurs after the product reaches RTM.  Ex: In the build name “7100.0.winmain_win7rc.090421-1700-3”, the DashNumber would be 3.  When the DashNumber is 0, it is omitted from the build name.</param>
		public BuildLabel
		(
			string branchName,
			Int16 buildNumber,
			Int16 buildQfe,
			string buildRevision,
			Int16 dashNumber = 0 )
		{
			Debug.Assert( !string.IsNullOrWhiteSpace( branchName ) );
			Debug.Assert( !string.IsNullOrWhiteSpace( buildRevision ) );

			this.branchName = branchName;
			this.buildNumber = buildNumber;
			this.buildQfe = buildQfe;
			this.buildRevision = buildRevision;
			this.dashNumber = dashNumber;
		}

		#endregion

		#region Properties

		public string BranchName
		{
			get { return this.branchName; }
		}

		/// <summary>
		/// Also known as Build Version
		/// </summary>
		public Int16 BuildNumber
		{
			get { return this.buildNumber; }
		}

		public Int16 BuildQfe
		{
			get { return this.buildQfe; }
		}

		public string BuildRevision
		{
			get { return this.buildRevision; }
		}

		public Int16 DashNumber
		{
			get { return this.dashNumber; }
		}

		public Int32 WinBuildID
		{
			get { return this.GetHashCode(); }
		}

		#endregion
	
		#region Overrides

		public override string ToString()
		{
			return string.Format
			(
				"{0}_{1}_{2}_{3}",
				this.BranchName,
				this.BuildNumber,
				this.BuildQfe,
				this.BuildRevision
			);
		}

		public override Int32 GetHashCode()
		{
			return KeyGen.WinBuildID
			(
				this.buildNumber,
				this.buildQfe,
				this.buildRevision ?? string.Empty,
				this.branchName
			);
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false
			if( obj == null )
			{
				return false;
			}

			BuildLabel other = obj as BuildLabel;

			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return this.GetHashCode() == other.GetHashCode();
		}

		#endregion

		#region Public Statics

		/// <summary>
		/// Returns a BuildLabel object for a source depot label syntax or Official Build Name syntax string
		/// </summary>
		/// <param name="buildLabelOrBuildName">
		/// Can be the source depot label syntax (e.g., "[branch]_7792_0_100802-1750" or
		/// "winblue_gdr_9600_16442_131022-1819" or "winmain_9889_0_141114-1920") or the build
		/// lab Official Build Name syntax, version.qfe.flavor.branch.revision (e.g, 
		/// "5456.0.amd64fre.vbl_tools_build.060614-1215" or "10240.0.winmain.150709-1450");
		/// see http://windowssites/sites/winbuilddocs/Wiki%20Pages/FindBuild%20Web%20Service.aspx
		/// </param>
		/// <returns>A BuildLabel object if the input string is nonnull and valid, NULL otherwise</returns>
		public static BuildLabel Parse( string buildLabelOrBuildName )
		{
			BuildLabel ret = null;

			if( !string.IsNullOrWhiteSpace( buildLabelOrBuildName ) )
			{
				Match buildLabelMatch = CommonRegex.BuildLabelRegex.Match( buildLabelOrBuildName );

				if( buildLabelMatch.Success )
				{
					ret = new BuildLabel
					(
						buildLabelMatch.Groups[ 1 ].ToString(),
						Int16.Parse( buildLabelMatch.Groups[ 2 ].ToString() ),
						Int16.Parse( buildLabelMatch.Groups[ 3 ].ToString() ),
						buildLabelMatch.Groups[ 4 ].ToString()
					);
				}
				else
				{
					Match buildNameMatch = CommonRegex.BuildNameRegex.Match( buildLabelOrBuildName );

					if( buildNameMatch.Success )
					{
						string flavor = buildNameMatch.Groups[ 3 ].ToString(); // TODO Pri 2: expose this?

						ret = new BuildLabel
						(
							buildNameMatch.Groups[ 3 ].ToString(),
							Int16.Parse( buildNameMatch.Groups[ 1 ].ToString() ),
							Int16.Parse( buildNameMatch.Groups[ 2 ].ToString() ),
							buildNameMatch.Groups[ 4 ].ToString()
						);
					}
				}
			}

			return ret;
		}

		public static Boolean operator ==( BuildLabel left, BuildLabel right )
		{
			Boolean equal = false;

			if( Object.Equals( left, null ) && Object.Equals( right, null ) )
			{
				equal = true;
			}
			else if
			(
				!Object.Equals( right, null )
				&& !Object.Equals( left, null )
				&& left.WinBuildID == right.WinBuildID )
			{
				equal = true;
			}

			return equal;
		}
	
		public static Boolean operator !=( BuildLabel left, BuildLabel right )
		{
			return !( left == right );
		}

		public static implicit operator string( BuildLabel buildLabel )
		{
			return buildLabel.ToString();
		}

		public static implicit operator BuildLabel( string buildLabelString )
		{
			BuildLabel ret = null;
			
			if( CommonRegex.BuildLabelRegex.IsMatch( buildLabelString ) )
			{
				ret = BuildLabel.Parse( buildLabelString );
			}

			return ret;
		}

		#endregion

		#region Converters

		public sealed class BuildLabelConverter : ConfigurationConverterBase
		{
			public Boolean ValidateType( object value, Type expected )
			{
				return !( value != null && value.GetType() != expected );
			}

			/// <summary>
			/// This method overrides CanConvertTo from TypeConverter. This is called
			/// when someone wants to convert an instance of BuildLabel to another type.
			/// Here, only conversion to an InstanceDescriptor is supported. 
			/// </summary>
			/// <param name="context"></param>
			/// <param name="destinationType"></param>
			/// <returns></returns>
			public override Boolean CanConvertTo( ITypeDescriptorContext context, Type destinationType )
			{
				if( destinationType == typeof( InstanceDescriptor ) )
				{
					return true;
				}

				// Always call the base to see if it can perform the conversion. 
				return base.CanConvertTo( context, destinationType );
			}

			public override Boolean CanConvertFrom( ITypeDescriptorContext ctx, Type type )
			{
				if( type == typeof( string ) || type == typeof( BuildLabel ) )
				{
					return true;
				}
				else
				{
					return base.CanConvertFrom( ctx, type );
				}
			}

			/// <summary>
			/// This code performs the actual conversion from a BuildLabel to an InstanceDescriptor. 
			/// </summary>
			public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
			{
				if( destinationType == typeof( InstanceDescriptor ) )
				{
					ConstructorInfo ci = typeof( BuildLabel ).GetConstructor
					(
						new Type[] { typeof( string ) }
					);

					BuildLabel t = ( BuildLabel )value;

					return new InstanceDescriptor( ci, new object[] { t.ToString() } );
				}

				// Always call base, even if you can't convert. 
				return base.ConvertTo( context, culture, value, destinationType );
			}

			public override object ConvertFrom
			(
				ITypeDescriptorContext ctx,
				CultureInfo ci,
				object data )
			{
				BuildLabel vfp = null;

				if( data != null )
				{
					vfp = BuildLabel.Parse( data.ToString() );
				}

				return vfp;
			}
		}

		#endregion
	}
}