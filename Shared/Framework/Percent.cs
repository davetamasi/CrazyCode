using System;
using System.Diagnostics;

namespace Tamasi.Shared.Framework.Types
{
	public struct Percent
	{
		#region Private Members and Constructors

		private float _percent;  // between 00.0000... and 100.0000...

		public Percent( Int32 part, Int32 total, Boolean throwOnNot0To100 = true )
		{
			if( total == 0 )
				_percent = 0;
			else
				_percent = 100 * ( float )part / ( float )total;

			if( _percent < 0 || _percent > 100 && throwOnNot0To100 )
			{
				throw new ArgumentException( "Percentage must be within [0,100]" );
			}
		}

		public Percent( float part, Int32 total, Boolean throwOnNot0To100 = true )
		{
			if( total == 0 )
			{
				_percent = 0;
			}
			else
			{
				_percent = 100 * ( float )part / ( float )total;
			}

			if( _percent < 0 || _percent > 100 && throwOnNot0To100 )
			{
				throw new ArgumentException( "Percentage must be within [0,100]" );
			}
		}

		public Percent( float floatVal, Boolean throwOnNot0To100 = true )
		{
			if( floatVal < 0f || floatVal > 1f && throwOnNot0To100 )
			{
				throw new ArgumentException( "Percentage must be within [0,100]" );
			}
			_percent = floatVal * 100;
		}

		#endregion

		public Int32 AsInt
		{
			get { return ( Int32 )Math.Abs( _percent ); }
		}

		/// <summary>
		/// Returns the percent value as a Byte (0-100)
		/// </summary>
		public Byte AsByte
		{
			get
			{
				Byte val = ( Byte )Math.Abs( _percent );
				Debug.Assert( val <= 100 );
				return val;
			}
		}

		public override string ToString()
		{
			return AsInt.ToString();
		}

		public override Boolean Equals( object obj )
		{
			// Check for null values and compare run-time types
			if( obj == null || this.GetType() != obj.GetType() )
			{
				return false;
			}

			Percent percent = ( Percent )obj;
			return percent._percent == this._percent;
		}

		public override Int32 GetHashCode()
		{
			return _percent.GetHashCode();
		}

		#region Statics

		public static implicit operator Percent( Int32 integer )
		{
			return new Percent( integer );
		}

		public static Percent Parse( object floatVal )
		{
			string stringVal = null;
			if( floatVal != null )
			{
				stringVal = floatVal.ToString();
			}
			return Parse( stringVal );
		}

		public static Percent Parse( string floatVal )
		{
			if( string.IsNullOrEmpty( floatVal ) )
			{
				return new Percent( 0 );
			}
			else
			{
				float f = float.Parse( floatVal );
				return new Percent( f ); // Assume we're parsing a float, so we need to move the decimal pt
			}
		}

		public static Int32 FindPercentBetween( Int32 value1, Int32 value2, Percent percent )
		{
			float oneHundredth = ( float )( value2 - value1 ) / 100;
			float addition = oneHundredth * ( float )percent.AsInt; // If percent=0, the startColor is returned
			Int32 newVal = value1 + ( Int32 )addition;
			return newVal;
		}

		public static readonly Percent OneHundred = new Percent( 1f );
		public static readonly Percent Eighty = new Percent( 0.8f );
		public static readonly Percent Fifty = new Percent( 0.5f );
		public static readonly Percent Twenty = new Percent( 0.2f );
		public static readonly Percent Zero = new Percent( 0f );

		public static Boolean operator ==( Percent x, Percent y )
		{
			return x.Equals( y );
		}

		public static Boolean operator !=( Percent x, Percent y )
		{
			return !( x == y );
		}

		public static Boolean operator >( Percent x, Percent y )
		{
			return x._percent > y._percent;
		}

		public static Boolean operator <( Percent x, Percent y )
		{
			return x._percent < y._percent;
		}

		#endregion
	}
}