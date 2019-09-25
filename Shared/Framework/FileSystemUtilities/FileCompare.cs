using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	/// <summary>
	/// This implementation defines a very simple comparison between two FileInfo objects. It only
	/// compares the name of the files being compared and their length in bytes
	/// </summary>
	public class FileCompare : IEqualityComparer<FileInfo>
	{
		public FileCompare()
		{
		}

		public Boolean Equals( System.IO.FileInfo f1, System.IO.FileInfo f2 )
		{
			return ( f1.Name == f2.Name &&
					f1.Length == f2.Length );
		}

		// Return a hash that reflects the comparison criteria. According to the rules for
		// IEqualityComparer<T>, if Equals is true, then the hash codes must also be equal. Because
		// equality as defined here is a simple value equality, not reference identity, it is
		// possible that two or more objects will produce the same hash code.
		public Int32 GetHashCode( FileInfo fi )
		{
			string s = String.Format( "{0}{1}", fi.Name, fi.Length );
			return s.GetHashCode();
		}

		#region Statics

		public static Boolean AreSameBinaryViaFullScan( FileInfo fileName1, FileInfo fileName2 )
		{
			// Check the file size and CRC equality here.. if they are equal...
			using( FileStream stream1 = fileName1.Open( FileMode.Open, FileAccess.Read, FileShare.Read ) )
			using( FileStream stream2 = fileName2.Open( FileMode.Open, FileAccess.Read, FileShare.Read ) )
			{
				return StreamEquals( stream1, stream2 );
			}
		}

		private static Boolean StreamEquals( Stream stream1, Stream stream2 )
		{
			const Int32 bufferSize = 2048;
			Byte[] buffer1 = new Byte[ bufferSize ]; //buffer size
			Byte[] buffer2 = new Byte[ bufferSize ];
			while( true )
			{
				Int32 count1 = stream1.Read( buffer1, 0, bufferSize );
				Int32 count2 = stream2.Read( buffer2, 0, bufferSize );

				if( count1 != count2 )
					return false;

				if( count1 == 0 )
					return true;

				// You might replace the following with an efficient "memcmp"
				if( !buffer1.Take( count1 ).SequenceEqual( buffer2.Take( count2 ) ) )
				{
					return false;
				}
			}
		}

		#endregion
	}
}