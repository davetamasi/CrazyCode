using System;

namespace Tamasi.Shared.WinFramework
{
	public static class Common
	{
		#region String Helper Methods

		public static Int16? ConvertStringWinBuildNumber( String buildNumberString )
		{
			Int16? ret = null;

			if( !String.IsNullOrEmpty( buildNumberString ) )
			{
				if( buildNumberString.Contains( "." ) )
				{
					buildNumberString = buildNumberString.Split( new char[] { '.' } )[ 0 ];
				}

				Int16 test = -1;
				if( Int16.TryParse( buildNumberString, out test ) )
				{
					ret = test;
				}
			}

			return ret;
		}

		#endregion
	}
}