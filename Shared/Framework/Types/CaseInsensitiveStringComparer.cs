using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamasi.Shared.Framework.Types
{
	public class CaseInsensitiveStringComparer : IEqualityComparer<string>
	{
		public Boolean Equals( string first, string second )
		{
			return 0 == string.Compare( first, second, true );
		}

		public Int32 GetHashCode( string first )
		{
			if( string.IsNullOrEmpty( first ) )
			{
				return 0;
			}
			else
			{
				return first.ToString().ToLower().GetHashCode();
			}
		}
	}
}
