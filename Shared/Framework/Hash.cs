using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tamasi.Shared.Framework
{
	public static class Hash
	{
		/// <summary>
		/// Scarab/Pandora string hash
		/// </summary>
		/// <param name="stringToHash">The string to hash</param>
		/// <param name="unicode">
		/// Whether to convert the string to unicode bytes (false) or ASCII bytes (true, default)
		/// </param>
		/// <param name="alg">SHA1 (default) or MD5</param>
		/// <returns>The first 64 bits of the SHA-1 (default) or MD5 hash</returns>
		public static Int64 HashString64( string stringToHash, Boolean unicode = false, Algorithm alg = Algorithm.SHA1 )
		{
			if( stringToHash == null )
			{
				throw new ArgumentNullException( nameof( stringToHash ) );
			}
			else if( stringToHash == string.Empty )
			{
				return 0;
			}
			else if( alg == Algorithm.SHA1 )
			{
				if( unicode )
				{
					return SHA1UnicodeHashWorker( stringToHash );
				}
				else
				{
					return SHA1HashWorker( stringToHash );
				}
			}
			else
			{
				if( unicode )
				{
					return MD5UnicodeHashWorker( stringToHash );
				}
				else
				{
					return MD5HashWorker( stringToHash );
				}
			}
		}

		/// <summary>
		/// A very weak hash (only 32bits) intended for use as a pre-calculated identifier; odds of
		/// collision are 1 in 4 billion so don't use for a set of objects that's too large
		/// </summary>
		/// <param name="stringToHash">The string to hash</param>
		/// <param name="alg">SHA1 (default) or MD5</param>
		/// <returns>The first 32 bits of the SHA-1 hash</returns>
		public static Int32 HashString32( string stringToHash, Boolean unicode = false, Algorithm alg = Algorithm.SHA1 )
		{
			return ( Int32 )( HashString64( stringToHash, unicode, alg ) );
		}

		/// <summary>
		/// Scarab/Pandora file hash
		/// </summary>
		/// <param name="file">The FileInfo object for the file to be hashed</param>
		/// <param name="alg">MD5 (default) or SHA1</param>
		/// <returns>The first 64 bits of the MD5 (default) or SHA1 hash</returns>
		public static Int64 HashFile
		(
			FileInfo file,
			Boolean unicode = false,
			Algorithm alg = Algorithm.MD5 )
		{
			Debug.Assert( file != null );

			if( !file.Exists )
			{
				throw new ArgumentException( "File to hash not found" );
			}

			// Convert the input file to a Byte array
			Byte[] fileInBytes = new Byte[ file.Length ];
			FileStream fs = null;

			using( fs = new FileStream( file.FullName, FileMode.Open, FileAccess.Read ) )
			{
				// Read block of bytes from stream into the Byte array
				fs.Read( fileInBytes, 0, System.Convert.ToInt32( fs.Length ) );
			}

			if( alg == Algorithm.MD5 )
			{
				return MD5HashWorker( fileInBytes );
			}
			else
			{
				return SHA1HashWorker( fileInBytes );
			}
		}

		/// <summary>
		/// Scarab/Pandora file hash, including isBinary
		/// </summary>
		/// <param name="file">The FileInfo object for the file to be hashed</param>
		/// <param name="isBinary">Whether the file is binary (via the MZ check)</param>
		/// <param name="alg">MD5 (default) or SHA1</param>
		/// <returns>The first 64 bits of the MD5 (default) or SHA1 hash</returns>
		public static Int64 HashFile
		(
			FileInfo file,
			out Boolean isBinary,
			Boolean unicode = false,
			Algorithm alg = Algorithm.MD5 )
		{
			Debug.Assert( file != null );

			if( !file.Exists )
			{
				throw new ArgumentException( "File to hash not found" );
			}

			// Convert the input file to a Byte array
			Byte[] fileInBytes = new Byte[ file.Length ];

			using( FileStream fs = new FileStream( file.FullName, FileMode.Open, FileAccess.Read ) )
			{
				// Read block of bytes from stream into the Byte array
				fs.Read( fileInBytes, 0, System.Convert.ToInt32( fs.Length ) );
			}

			// Check for MZ at the beginning
			isBinary = fileInBytes.LongLength > 2 && fileInBytes[ 0 ] == 'M' && fileInBytes[ 1 ] == 'Z';

			if( alg == Algorithm.MD5 )
			{
				return MD5HashWorker( fileInBytes );
			}
			else
			{
				return SHA1HashWorker( fileInBytes );
			}
		}

		public enum Algorithm
		{
			SHA1,
			MD5
		}

		#region Privates

		private static Int64 MD5HashWorker( Byte[] input )
		{
			Byte[] hashCode = null;

			using( MD5Cng md5 = new MD5Cng() )
			{
				hashCode = md5.ComputeHash( input );
			}

			return BitConverter.ToInt64( hashCode, 0 );
		}

		private static Int64 SHA1HashWorker( Byte[] input )
		{
			Byte[] hashCode = null;

			using( SHA1Cng sha = new SHA1Cng() )
			{
				hashCode = sha.ComputeHash( input );
			}

			return BitConverter.ToInt64( hashCode, 0 );
		}

		private static Int64 MD5HashWorker( string input )
		{
			Byte[] hashCode = null;

			Byte[] inputBytes = Common.StringToByteArray( input );

			using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() )
			{
				hashCode = md5.ComputeHash( inputBytes );
			}

			return BitConverter.ToInt64( hashCode, 0 );
		}

		private static Int64 MD5UnicodeHashWorker( string input )
		{
			Byte[] hashCode = null;

			UnicodeEncoding UE = new UnicodeEncoding();
			Byte[] inputBytes = UE.GetBytes( input );

			using( MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider() )
			{
				hashCode = md5.ComputeHash( inputBytes );
			}

			return BitConverter.ToInt64( hashCode, 0 );
		}

		private static Int64 SHA1HashWorker( string input )
		{
			Byte[] hashCode = null;

			Byte[] inputBytes = Common.StringToByteArray( input );

			using( SHA1Managed sha1 = new SHA1Managed() )
			{
				hashCode = sha1.ComputeHash( inputBytes );
			}

			return BitConverter.ToInt64( hashCode, 0 );
		}

		private static Int64 SHA1UnicodeHashWorker( string input )
		{
			Byte[] hashCode = null;

			UnicodeEncoding UE = new UnicodeEncoding();
			Byte[] inputBytes = UE.GetBytes( input );

			using( SHA1Managed sha1 = new SHA1Managed() )
			{
				hashCode = sha1.ComputeHash( inputBytes );
			}

			return BitConverter.ToInt64( hashCode, 0 );
		}

		#endregion
	}
}