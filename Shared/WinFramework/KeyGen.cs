using System;
using System.Diagnostics;

using Tamasi.Shared.Framework;
using Tamasi.Shared.WinFramework.Types;

namespace Tamasi.Shared.WinFramework
{
	public static class KeyGen
	{
		/// <summary>
		/// Implements the key generation logic in [base].[KeyGen_DepotID]
		/// </summary>
		public static Int32 DepotID( string depotName )
		{
			Int32 depotID = 0;
	
			//RETURN CASE WHEN ( @depotName IS NULL OR LEN(@depotName)=0 ) THEN 0
			//			ELSE [util].[HASH_SHA1_32]( UPPER( LTRIM( RTRIM( @depotName ) ) ) )
			//			END;

			if( !string.IsNullOrEmpty( depotName ) )
			{
				depotID = Hash.HashString32( depotName.Trim().ToUpper() );
			}

			return depotID;
		}

		/// <summary>
		/// Implements the key generation logic in [base].[KeyGen_ReleaseBinaryID]
		/// </summary>		
		public static Int64 ReleaseBinaryNameID( string binaryName, Int16 winReleaseID )
		{
			Int64 releaseBinaryID = 0;
			
			if( winReleaseID > 0 && !string.IsNullOrEmpty( binaryName ) )
			{
				string toHash = string.Format
				(
					"ReleaseBinaryID{0}{1}",
					binaryName.ToLower().Trim(),
					winReleaseID
				);

				releaseBinaryID = Hash.HashString64( toHash );
			}

			return releaseBinaryID;
		}

		public static Int32 BinaryNameID( string binaryName )
		{
			Int32 binaryNameID = 0;

			if( !string.IsNullOrEmpty( binaryName ) )
			{
				binaryNameID = Hash.HashString32( binaryName.ToLower().Trim() );
			}

			return binaryNameID;
		}

		public static Int32 WinBuildID( string buildLabel )
		{
			Int32 winBuildID = 0;

			if( !string.IsNullOrEmpty( buildLabel ) )
			{
				winBuildID = Hash.HashString32( buildLabel.ToLower() );
			}

			return winBuildID;
		}

		public static Int32 WinBuildID( Int16 buildNumber, Int16 buildQfe, string buildRevision, string branchName )
		{
			Int32 winBuildID = 0;

			if( buildNumber > 0 && !string.IsNullOrEmpty( buildRevision ) && !string.IsNullOrEmpty( branchName ) )
			{
				// Construct the build label. e./g./, win7_rtm_7600_16385_090713-1255
				string hash = string.Format
				(
					"{0}_{1}_{2}_{3}",
					branchName.ToLower(),
					buildNumber,
					buildQfe,
					buildRevision.ToLower()
				);

				winBuildID = Hash.HashString32( hash );
			}

			return winBuildID;
		}

		public static Int32 BuildFlavorID( string procArchString, string chunkName, string buildTypeString )
		{
			Int32 buildFlavorID = 0;

			if( !string.IsNullOrEmpty( procArchString ) && !string.IsNullOrEmpty( chunkName ) && !string.IsNullOrEmpty( buildTypeString ) )
			{
				// Make sure we converted the enum string value to en-us
				Debug.Assert( !chunkName.Contains( "en_us" ) );

				string hash = string.Format
				(
					"{0}{1}{2}",
					chunkName.ToLower(),
					procArchString.ToLower(),
					buildTypeString.ToLower()
				);

				buildFlavorID = Hash.HashString32( hash );
			}

			return buildFlavorID;
		}

		/// <summary>
		/// Generates the BuildInstanceID (analagous to the shared [base].[KeyGen_BuildInstanceID] SQL function)
		/// </summary>
		/// <param name="buildLabel"></param>
		/// <param name="targetFlavor"></param>
		/// <returns></returns>
		public static Int32 BuildInstanceID( BuildLabel buildLabel, TargetBuildFlavorBase targetFlavor )
		{
			Int32 buildInstanceID = 0;

			if( buildLabel != null && targetFlavor != null && buildLabel.WinBuildID != 0 && targetFlavor.BuildFlavorID != 0 )
			{
				string hash = string.Format
				(
					"!WinBuild{0}!BuildFlavor{1}",
					buildLabel.WinBuildID,
					targetFlavor.BuildFlavorID
				);

				buildInstanceID = Hash.HashString32( hash );
			}

			return buildInstanceID;
		}

		public static Int32 ProductBinaryPathID( string productBinaryPath )
		{
			Int32 productBinaryPathID = 0;

			if( productBinaryPath != null && productBinaryPath.StartsWith( @"\" ) ) // TODO Pri 2: Implement [base].[IsValidProductBinaryPath]
			{
				string hash = string.Format
				(
					"ProductBinaryPathID!{0}",
					productBinaryPath.ToLower().Trim()
				);

				productBinaryPathID = Hash.HashString32( hash );
			}

			return productBinaryPathID;
		}
    }
}
