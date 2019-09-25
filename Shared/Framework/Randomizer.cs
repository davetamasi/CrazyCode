using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tamasi.Shared.Framework
{
	public static class Randomizer
	{
		#region Random Integers

		/// <summary>
		/// Generates a random Int16 with optional lower bound and optional upper bound
		/// </summary>
		public static Int16 Int16( Int16 lowerBound = short.MinValue, Int16 upperBound = short.MaxValue )
		{
			Int16 range = ( Int16 )( upperBound - lowerBound );
			Int16 offset = ( Int16 )( random.Next() % range );
			Int16 ret = ( Int16 )( lowerBound + offset );

			Debug.Assert( ret >= lowerBound && ret <= upperBound );

			return ret;
		}

		/// <summary>
		/// Generates a random integer with optional lower bound and optional upper bound
		/// </summary>
		public static Int32 Int32( Int32 lowerBound = int.MinValue, Int32 upperBound = int.MaxValue )
		{
			Int32 range = ( upperBound - lowerBound );
			Int32 offset = ( random.Next() % range );
			Int32 ret = ( lowerBound + offset );

			Debug.Assert( ret >= lowerBound && ret <= upperBound );

			return ret;
		}

		public static Int32 Int32( Int32 upperBound )
		{
			if( upperBound <= 0 )
			{
				return 0;
			}
			else
			{
				return Convert.ToInt32( random.Next() ) % upperBound;
			}
		}

		public static UInt64 UInt64( UInt64 upperBound )
		{
			if( upperBound <= 0 )
			{
				return 0;
			}
			else
			{
				return Convert.ToUInt64( random.Next() ) % upperBound;
			}
		}

		public static Int64 Int64()
		{
			return Convert.ToInt64( random.Next() );
		}

		public static UInt64 UInt64()
		{
			return Convert.ToUInt64( random.Next() );
		}

		#endregion

		#region Random Dates, Bools, and Strings

		public static DateTime RandomDateTime()
		{
			return DateTime.FromBinary( Int64() );
		}

		public static DateTime? NullableDateTime()
		{
			DateTime? ret = null;

			switch( RandomBool() )
			{
				case true:
					break;

				case false:
					ret = RandomDateTime();
					break;
			}

			return ret;
		}

		public static Boolean RandomBool()
		{
			return 1 == random.Next( 2 );
		}

		public static Boolean? NullableBool()
		{
			Boolean? ret = null;

			switch( random.Next( 3 ) )
			{
				case 0:
					break;

				case 1:
					ret = true;
					break;

				case 2:
					ret = false;
					break;
			}

			return ret;
		}

		/// <summary>
		/// Returns string of given (max) length, or of random length up to 65335 characters
		/// </summary>
		/// <param name="length">Either max or exact length of the string</param>
		/// <param name="varyLength">if TRUE, vary length, else exact</param>
		public static string String
		(
			Int32 stringLength = 65335,
			Boolean varyLength = true,
			Boolean unicode = false )
		{
			Int32 length = stringLength;

			if( varyLength )
			{
				length = Int32( stringLength );
			}

			if( unicode )
			{
				Byte[] str = new Byte[ length * 2 ];

				for( Int32 i = 0; i < length * 2; i += 2 )
				{
					Int32 chr = random.Next( 0xD7FF );
					str[ i + 1 ] = ( Byte )( ( chr & 0xFF00 ) >> 8 );
					str[ i ] = ( Byte )( chr & 0xFF );
				}

				return Encoding.Unicode.GetString( str );
			}
			else
			{
				StringBuilder sb = new StringBuilder();

				for( Int32 k = 0; k < length; )
				{
					Int32 ascii = random.Next( 100 ) + 16;
					if( ascii > 32 && ascii < 128 )
					{
						k++;
						char c = ( char )ascii;
						sb.Append( c.ToString() );
					}
				}

				return sb.ToString();
			}
		}

		#endregion

		#region Collection Randomness

		public static T RandomEnumValue<T>()
		{
			Array values = Enum.GetValues( typeof( T ) );
			Int32 randomIndex = Int32( values.GetLength( 0 ) );
			return ( T )values.GetValue( randomIndex );
		}

		public static T RandomObjectFromPool<T>( IList<T> pool )
		{
			Debug.Assert( pool != null );

			if( pool.Count == 0 )
			{
				return default( T );
			}
			else
			{
				return pool[ Int32( pool.Count ) ];
			}
		}

		#endregion

		#region Privates

		private static Random random = new Random();

		private static UInt32 GetUInt()
		{
			Int32 nonNegative = random.Next();
			Boolean shift = random.Next( 2 ) == 0;
			// This is now full range
			Int32 value = shift ? nonNegative - int.MaxValue : nonNegative;

			UInt32 unsigned = unchecked( ( UInt32 )value );
			return unsigned;
		}

		#endregion
	}
}