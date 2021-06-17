using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

using Tamasi.Shared.Framework.Types;

namespace Tamasi.Shared.Framework
{
	/// <summary>
	/// Common helper functions, can be included just by itself if other projects, so don't link to
	/// anything outside the standard .NET framework and this one file
	/// </summary>
	public static class Common
	{
		#region Sequence and Member Functions

		public static Boolean IsMember( String[] array, String thing, Boolean caseInsensitive )
		{
			Boolean retVal = false;
			foreach( String member in array )
			{
				if( caseInsensitive )
					retVal = ( 0 == String.Compare( member, thing, true ) );
				else
					retVal = member == thing;

				if( retVal )
					break;
			}
			return retVal;
		}

		public static Boolean IsMember<T>( T[] array, T thing )
		{
			Boolean retVal = false;
			foreach( T member in array )
			{
				retVal = object.Equals( member, thing );
				if( retVal )
					break;
			}
			return retVal;
		}

		public static Boolean ArrayIncludesString( String[] array, String inputStr )
		{
			return ArrayIncludesString( array, inputStr, false );
		}

		public static Boolean ArrayIncludesString( String[] array, String inputStr, Boolean ignoreCase )
		{
			if( null == array || inputStr == null )
				return false;

			foreach( String item in array )
			{
				if( !String.IsNullOrEmpty( item ) && 0 == String.Compare( inputStr, item, ignoreCase ) )
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if the list contains the item
		/// </summary>
		public static Boolean IsMember<T>( List<T> list, T item )
		{
			Boolean retVal = false;
			foreach( T member in list )
			{
				retVal = object.Equals( member, item );
				if( retVal )
					break;
			}
			return retVal;
		}

		#endregion Sequence and Member Functions

		#region Array Methods

		/// <summary>
		/// Returns the first nonnull, nonempty String from the item list; returns null if none are
		/// (similar to SQL Server)
		/// </summary>
		public static String Coalesce( params object[] items )
		{
			String ret = null;

			foreach( object item in items )
			{
				if( item != null && !String.IsNullOrEmpty( item.ToString() ) )
				{
					ret = item.ToString();
					break;
				}
			}

			return ret;
		}

		public static T[] CollapseArrays<T>( T[][] arrayOfArrays )
		{
			List<T> list = new List<T>();  // Don't know the target size in advance
			foreach( T[] array in arrayOfArrays )
			{
				foreach( T element in array )
				{
					// Remove dupes
					if( !list.Contains( element ) )
						list.Add( element );
				}
			}
			list.Sort();
			return list.ToArray();
		}

		public static String[] StringArrayFromDataView( DataView dataView, Int32 column )
		{
			List<String> temp = new List<String>();
			using( dataView )
			{
				foreach( DataRowView dataRowView in dataView )
				{
					String val = dataRowView[ column ].ToString();
					temp.Add( val );
				}
			}
			return temp.ToArray();
		}

		public static Int32[] IntArrayFromDataView( DataView dataView, Int32 column )
		{
			List<Int32> temp = new List<Int32>( dataView.Count );
			foreach( DataRowView dataRowView in dataView )
			{
				Int32 val;
				// Theoretically, might be a null in there
				if( dataRowView[ column ] != null && Int32.TryParse( dataRowView[ column ].ToString(), out val ) )
				{
					temp.Add( val );
				}
			}
			return temp.ToArray();
		}

		public static String[] ConvertToStringArray<T>( T[] array )
		{
			Debug.Assert( array != null );

			List<String> temp = new List<String>( array.GetLength( 0 ) );
			foreach( T item in array )
			{
				temp.Add( item.ToString() );
			}
			return temp.ToArray();
		}

		#endregion Array Methods

		#region Collection Methods

		public static String CollapseList( IList<String> listOfStrings )
		{
			Debug.Assert( listOfStrings != null );

			StringBuilder sb = new StringBuilder();

			foreach( String stringItem in listOfStrings )
			{
				sb.AppendFormat( @"{0}\r\n", stringItem );
			}

			return sb.ToString();
		}

		public static StringCollection ConvertToStringCollection( String[] stringArray )
		{
			Debug.Assert( stringArray != null );

			StringCollection stringCollection = new StringCollection();
			stringCollection.AddRange( stringArray );
			return stringCollection;
		}

		/// <summary>
		/// Splits the collection evenly up into Lists, with no List.Count &gt; maxElements
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		public static ReadOnlyCollection<ReadOnlyCollection<T>> Partition<T>
		(
			IList<T> collection,
			Int32 maxElements )
		{
			Int32 remainder; // discard
			Int32 listCount = 1 + Math.DivRem( collection.Count, maxElements, out remainder );
			Int32 listSize = 1 + Math.DivRem( collection.Count, listCount, out remainder );

			List<T>[] lists = new List<T>[ listCount ];

			for( Int32 k = 0; k < listCount; k++ )
			{
				lists[ k ] = new List<T>( listSize );
			}

			for( Int32 k = 0; k < collection.Count; k++ )
			{
				lists[ k % listCount ].Add( collection[ k ] );
			}

			return lists.Select( l => l.ToList().AsReadOnly() ).ToList().AsReadOnly();
		}

		/// <summary>
		/// Compares two lists of the same type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <param name="onlyInFirst"></param>
		/// <param name="onlyinSecond"></param>
		/// <param name="inBoth"></param>
		/// <param name="comparer"></param>
		/// <returns>TRUE if the lists are equal (though not necessarily in the same order)</returns>
		public static Boolean CompareLists<T>
		(
			IEnumerable<T> first,
			IEnumerable<T> second,
			out IEnumerable<T> onlyInFirst,
			out IEnumerable<T> onlyinSecond,
			out IEnumerable<T> inBoth,
			IEqualityComparer<T> comparer = null )
		{
			onlyInFirst = first.Except( second, comparer ).AsEnumerable();
			onlyinSecond = second.Except( first, comparer ).AsEnumerable();
			inBoth = first.Intersect( second, comparer ).AsEnumerable();

			return onlyInFirst.Count() == 0 && onlyinSecond.Count() == 0;
		}

		#endregion Collection Methods

		#region Console Output Formatters

		public static object DrawSyncRoot = new object();
		public static StringBuilder CompletionMsg = new StringBuilder();

		/// <summary>
		/// Write message to the console (and maybe log eventually) in the default color
		/// </summary>
		public static void WriteLine( String format, params object[] args )
		{
			String message = ( args != null ) ? String.Format( format, args ) : format;
			Console.WriteLine
			(
				"[{0:00}|{1:HH:mm:ss}]-> {2}",
				Thread.CurrentThread.ManagedThreadId,
				DateTime.Now,
				message
			);
		}

		/// <summary>
		/// Write message to the console (and maybe log eventually) with a particular color
		/// </summary>
		public static void WriteLine( ConsoleColor color, String format, params object[] args )
		{
			String message = ( args != null ) ? String.Format( format, args ) : format;

			lock ( DrawSyncRoot )
			{
				Console.ForegroundColor = color;
				Console.WriteLine
				(
					"[{0:00}|{1:HH:mm:ss}]-> {2}",
					Thread.CurrentThread.ManagedThreadId,
					DateTime.Now,
					message
				);
				Console.ResetColor();
			}
		}

		/// <summary>
		/// Write to the console (and maybe log eventually), indicating whether message should be
		/// printed based on verbosity
		/// </summary>
		public static void WriteVerboseLine( String format, params object[] args )
		{
			String message = ( args != null ) ? String.Format( format, args ) : format;

			lock ( DrawSyncRoot )
			{
				Console.ForegroundColor = ConsoleColor.DarkGray;
				Console.WriteLine
				(
					"[{0:00}|{1:HH:mm:ss} V]-> {2}",
					Thread.CurrentThread.ManagedThreadId,
					DateTime.Now,
					message
				);
				Console.ResetColor();
			}
			// TODO Pri 2: Implement ScarabSettings.Current to be able to toggle showing this If not
			// shown, then do this: Debug.WriteLine( "VERBOSE-> " + message );
		}

		/// <summary>
		/// Intended to announce a global failure
		/// </summary>
		public static void DrawUnhandledException( Exception exception, String format, params object[] args )
		{
			String message = String.Format( format, args );
			DrawUnhandledException( exception, message );
		}

		/// <summary>
		/// Intended to announce a global failure
		/// </summary>
		public static void DrawUnhandledException( Exception exception, String message = null )
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine( "--------------------------------------------------------------------------" );
			sb.AppendFormat( "! UNHANDLED EXCEPTION: {0}\r\n", message );
			sb.AppendFormat( "! TYPE:                {0}\r\n", exception.GetType().ToString() );
			sb.AppendFormat( "! INNER TYPE:          {0}\r\n", exception?.InnerException.GetType().ToString() );

			if( exception is AggregateException )
			{
				IList<Exception> ies = ( exception as AggregateException ).InnerExceptions;
				foreach( var ie in ies )
				{
					sb.AppendFormat( "! {0}\r\n", ie.Message );
					sb.AppendFormat( "! {0}\r\n", ie.StackTrace );
					sb.AppendLine( "--------------------------------------------------------------------------" );
				}
			}
			else
			{
				sb.AppendFormat( "! {0}\r\n", exception.Message );
				sb.AppendFormat( "! {0}\r\n", exception.StackTrace );
				sb.AppendLine( "--------------------------------------------------------------------------" );
			}

			lock ( Common.DrawSyncRoot )
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine( sb.ToString() );
				Console.ResetColor();
			}
		}

		public static void DrawInlineHighlight( ConsoleColor color, String format, params object[] args )
		{
			String message = String.Format( format, args );

			lock ( DrawSyncRoot )
			{
				Console.ForegroundColor = color;
				Console.Write( message );
				Console.ResetColor();
			}
		}

		public static void DrawWarning( String format, params object[] args )
		{
			Debug.Assert( format != null );

			String issue = ( args.Length > 0 )
				? String.Format( format, args )
				: format;

			String message = String.Format
			(
				"[{0:00}|{1:HH:mm:ss} WARNING]-> {2}",
				Thread.CurrentThread.ManagedThreadId,
				DateTime.Now,
				issue
			);

			lock ( Common.DrawSyncRoot )
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine( message );
				Console.ResetColor();
			}
		}

		#endregion Console Output Formatters

		#region String Helper Methods

		/// <summary>
		/// Used to compare strings where you want null==String.Empty
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Boolean CompareCoalescingEmptyStrings( String a, String b )
		{
			if( String.IsNullOrEmpty( a ) )
			{
				return String.IsNullOrEmpty( b );
			}
			else
			{
				return String.Equals( a, b );
			}
		}

		/// <summary>
		/// Similar but not quite the same as a file path (e.g., used for CompCentral)
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		public static String CreatePath( params String[] args )
		{
			StringBuilder sb = new StringBuilder();

			foreach( String arg in args )
			{
				if( !String.IsNullOrEmpty( arg ) )
				{
					sb.Append( @"\" ).Append( arg.ToString() );
				}
			}

			return sb.ToString();
		}

		/// <summary>
		/// Counts how many times char y appears in String x
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Int32 CountInstances( String x, char y )
		{
			Int32 count = 0;

			foreach( char x1 in x.ToCharArray() )
			{
				if( x1 == y ) count++;
			}
			return count;
		}

		/// <summary>
		/// Extension method to tests whether a String contains any of the given strings
		/// </summary>
		/// <returns>
		/// Returns TRUE if the testString contains any of the listed strings, FALSE otherwise
		/// </returns>
		public static Boolean ContainsAny
		(
			this String x,
			IEnumerable<String> stringValues,
			StringComparison comparisonType = StringComparison.CurrentCulture )
		{
			Boolean retval = false;

			foreach( String stringValue in stringValues )
			{
				if( x.IndexOf( stringValue, comparisonType ) >= 0 )
				{
					retval = true;
					break;
				}
			}

			return retval;
		}

		/// <summary>
		/// Extension method to tests whether a String starts with any of the given strings
		/// </summary>
		/// <returns>
		/// Returns TRUE if the testString starts with any of the listed strings, FALSE otherwise
		/// </returns>
		public static Boolean StartsWithAny
		(
			this String x,
			IEnumerable<String> stringValues,
			StringComparison comparisonType = StringComparison.CurrentCulture )
		{
			Boolean retval = false;

			foreach( String stringValue in stringValues )
			{
				if( x.StartsWith( stringValue, comparisonType ) )
				{
					retval = true;
					break;
				}
			}

			return retval;
		}

		/// <summary>
		/// Converts "fooBar" to "FooBar"
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static String UppercaseFirstLetter( String s )
		{
			if( String.IsNullOrEmpty( s ) )
			{
				return String.Empty;
			}

			return char.ToUpper( s[ 0 ] ) + s.Substring( 1 );
		}

		/// <summary>
		/// Returns the file's extension in all caps and no dot
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static String GetExtension( String filePath )
		{
			String extension = Path.GetExtension( filePath );

			if( String.IsNullOrEmpty( extension ) )
			{
				extension = null;
			}
			else
			{
				extension = extension.Replace( ".", String.Empty ).ToUpper();
			}

			return extension;
		}

		public static Byte[] StringToByteArray( String input )
		{
			Byte[] byteArray = new Byte[ input.Length ];

			for( Int32 i = 0; i < input.Length; i++ )
			{
				// in case we're passed Unicode strings here we mask off the upper Byte
				byteArray[ i ] = Convert.ToByte( input[ i ] & Byte.MaxValue );
			}

			return byteArray;
		}

		public static String ByteArrayToString( Byte[] bytes )
		{
			StringBuilder str = new StringBuilder();

			for( Int32 i = 0; i < bytes.Length; i++ )
			{
				str.AppendFormat( "{0:X2}", bytes[ i ] );
			}

			return str.ToString();
		}

		public static String TrimLastSingleCarriageReturn( String val )
		{
			// \n = CR (Carriage Return) // Used as a new line character in Unix \r = LF (Line Feed)
			// // Used as a new line character in Mac OS \n\r = CR + LF // Used as a new line
			// character in Windows (char)13 = \n = CR // Same as \n

			String ret = val;

			if( !String.IsNullOrEmpty( val ) )
			{
				if( val.EndsWith( "\r\n" ) )
				{
					ret = val.Remove( val.Length - 2, 2 );
				}
				else if( val.EndsWith( "\n" ) )
				{
					ret = val.Remove( val.Length - 1, 1 );
				}
			}

			return ret;
		}

		/// <summary>
		/// Remove all characters <0x20
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static String ScrubUnprintableChars( String s )
		{
			if( s == null )
			{
				return null;
			}

			StringBuilder sb = new StringBuilder();

			for( Int32 i = 0; i < s.Length; i++ )
			{
				char c = s[i];

				if( c >= 0x20 )
				{
					sb.Append( c );
				}
			}

			return sb.ToString();
		}

		#endregion

		#region Delimited List Helper Methods

		/// <summary>
		/// Turns an array of String-able values into: item1,item2,item3 (or
		/// 'item1','item2','item3', or whatever)
		/// </summary>
		public static String DelimitedList<T>( IList<T> list, String delimiter = ",", String wrapper = null )
		{
			Debug.Assert( list != null );

			var parameters = list.Select( x => String.Format( "{0}{1}{0}", wrapper, x ) ).ToArray();
			String delimitedList = String.Join( delimiter, parameters );

			return delimitedList;
		}

		/// <summary>
		/// Turns an array of String-able values into: item1,item2,item3
		/// </summary>
		public static String CommaDelimitedList<T>( T[] array, String delimiter, Boolean addSingleQuotes )
		{
			Debug.Assert( array != null );

			return DelimitedList<T>
			(
				array,
				delimiter,
				( addSingleQuotes ) ? "'" : null
			);
		}

		/// <summary>
		/// Turns an arbitrary array into:
		/// "element1.ToString(),element1.ToString(),element1.ToString(),..." and optionally adds
		/// single quote marks around the items in the last
		/// </summary>
		public static String CommaDelimitedList<T>( T[] array, Boolean addSingleQuotes )
		{
			return DelimitedList
			(
				array,
				wrapper: ( addSingleQuotes ) ? "'" : null
			);
		}

		/// <summary>
		/// Turns an arbitrary array into:
		/// "element1.ToString(),element1.ToString(),element1.ToString(),..." and optionally adds
		/// single quote marks around the items in the last
		/// </summary>
		public static String CommaDelimitedList<T>( IList<T> list, Boolean addSingleQuotes = false )
		{
			Debug.Assert( list != null );

			return DelimitedList
			(
				list,
				wrapper: ( addSingleQuotes ) ? "'" : null
			);
		}

		/// <summary>
		/// Returns a List of Strings from a CDL of strings
		/// </summary>
		/// <param name="dlString"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static List<String> StringListFromDelimitedString
		(
			String dlString,
			String delimiter = "," )
		{
			List<String> ret = new List<String>();

			if( !String.IsNullOrWhiteSpace( dlString ) && !String.IsNullOrWhiteSpace( delimiter ) )
			{
				String[] split = dlString.Split
				(
					new String[] { delimiter },
					StringSplitOptions.RemoveEmptyEntries
				);

				ret = split.ToList();
			}

			return ret;
		}

		/// <summary>
		/// Returns a list of Integers from a CDL of ints
		/// </summary>
		/// <param name="commaDelimitedIntList"></param>
		/// <param name="delimiter"></param>
		/// <returns></returns>
		public static List<Int32> IntListFromDelimitedString
		(
			String commaDelimitedIntList,
			String delimiter = "," )
		{
			String[] splitList = commaDelimitedIntList.Split
			(
				new String[] { "," },
				StringSplitOptions.RemoveEmptyEntries
			);

			List<Int32> ints = new List<Int32>( splitList.GetLength( 0 ) );

			foreach( String intString in splitList )
			{
				Int32 tempInt;
				if( Int32.TryParse( intString, out tempInt ) )
				{
					ints.Add( tempInt );
				}
			}

			return ints;
		}

		/// <summary>
		/// Constructs a comma-delimited String from the IEnumerable
		/// </summary>
		public static String CdlStringFromList<T>
		(
			IList<T> itemList,
			String separator = ",",
			Boolean includeEmpty = false )
		{
			if( itemList == null ) return null;

			StringBuilder sb = new StringBuilder();

			foreach( T item in itemList )
			{
				String itemString = item.ToString();
				if( !includeEmpty && String.IsNullOrEmpty( itemString ) )
				{
					continue;
				}
				sb.Append( itemString ).Append( separator );
			}

			String cdlString = sb.ToString();

			if( cdlString != String.Empty )
			{
				cdlString = cdlString.Substring( 0, cdlString.Length - separator.Length );
			}

			return cdlString;
		}

		/// <summary>
		/// Constructs a colon + semicolon delimited String from a dictionary, e.g., "key1:value1;key2:value2;"
		/// </summary>
		public static String CdlStringFromDictionary<T, R>
		(
			IDictionary<T, R> dictionary,
			Boolean includeEmpty = false )
		{
			if( dictionary == null ) return null;

			StringBuilder sb = new StringBuilder();

			foreach( T key in dictionary.Keys )
			{
				String keyString = key.ToString();
				String valueString = dictionary[ key ].ToString();

				if( !includeEmpty &&
					( String.IsNullOrEmpty( keyString )
						|| String.IsNullOrEmpty( valueString ) ) )
				{
					continue;
				}

				sb.AppendFormat( "{0}:{1};", keyString, valueString );
			}

			String cdlString = sb.ToString();

			return cdlString;
		}

		#endregion Delimitied List Helper Methods

		#region Text File Helper Methods

		public static String ReadTextFile( FileInfo textFile )
		{
			StringBuilder sb = new StringBuilder();

			using( TextReader reader = File.OpenText( textFile.FullName ) )
			{
				while( reader.Peek() > -1 )
				{
					sb.AppendLine( reader.ReadLine() );
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Creates a text file from a list of lines
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="lines"></param>
		/// <param name="targetFilePath"></param>
		public static void WriteTextFile
		(
			IList<String> lines,
			FileInfo targetFilePath,
			CollisionRemediation cr = CollisionRemediation.Throw )
		{
			Debug.Assert( lines != null );
			Debug.Assert( targetFilePath != null );

			Boolean proceed = true;
			Boolean throwEx = false;

			if( cr != CollisionRemediation.Passive && targetFilePath.Exists )
			{
				switch( cr )
				{
					case CollisionRemediation.Break:
					case CollisionRemediation.FillIn:
						proceed = false;
						break;

					case CollisionRemediation.Mirror:
					case CollisionRemediation.Overwrite:
						targetFilePath.Delete();
						break;

					case CollisionRemediation.Throw:
						throwEx = true;
						break;
				}
			}

			if( throwEx )
			{
				// TODO Pri 2: Why not ScarabException???
				throw new ApplicationException( String.Format
				(
					"File {0} already exists",
					targetFilePath.FullName
				) );
			}

			if( proceed )
			{
				using( TextWriter writer = File.CreateText( targetFilePath.FullName ) )
				{
					foreach( String line in lines )
					{
						writer.WriteLine( line );
					}
				}
			}
		}

		/// <summary>
		/// Creates a text file whose lines are the items, in order
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="targetFilePath"></param>
		public static void WriteTextFile
		(
			String text,
			FileInfo targetFilePath,
			CollisionRemediation cr = CollisionRemediation.Throw )
		{
			Debug.Assert( text != null );
			Debug.Assert( targetFilePath != null );

			Boolean proceed = true;
			Boolean throwEx = false;

			if( cr != CollisionRemediation.Passive && targetFilePath.Exists )
			{
				switch( cr )
				{
					case CollisionRemediation.Break:
					case CollisionRemediation.FillIn:
						proceed = false;
						break;

					case CollisionRemediation.Mirror:
					case CollisionRemediation.Overwrite:
						targetFilePath.Delete();
						break;

					case CollisionRemediation.Throw:
						throwEx = true;
						break;
				}
			}

			if( throwEx )
			{
				// TODO Pri 2: Why not ScarabException???
				throw new ApplicationException( String.Format
				(
					"File {0} already exists",
					targetFilePath.FullName
				) );
			}

			if( proceed )
			{
				using( TextWriter writer = File.CreateText( targetFilePath.FullName ) )
				{
					writer.WriteLine( text );
				}
			}
		}

		/// <summary>
		/// Reads a text file from disk and splits it into lines
		/// </summary>
		public static ReadOnlyCollection<String> ParseTextFile( FileInfo textFile )
		{
			List<String> lines = new List<String>();

			using( TextReader reader = File.OpenText( textFile.FullName ) )
			{
				while( reader.Peek() > -1 )
				{
					lines.Add( reader.ReadLine() );
				}
			}

			return new ReadOnlyCollection<String>( lines );
		}

		/// <summary>
		/// Reads a text file from disk
		/// </summary>
		/// <param name="textFile"></param>
		/// <param name="regex"></param>
		/// <returns>A Match collection of its lines that match the given regex</returns>
		public static ReadOnlyCollection<Match> ParseTextFile( FileInfo textFile, Regex regex )
		{
			List<Match> matches = new List<Match>();

			using( TextReader reader = File.OpenText( textFile.FullName ) )
			{
				while( reader.Peek() > -1 )
				{
					String line = reader.ReadLine();
					Match match = regex.Match( line );
					if( match.Success )
					{
						matches.Add( match );
					}
				}
			}

			return new ReadOnlyCollection<Match>( matches );
		}

		/// <summary>
		/// Splits on the Windows CR-LF first, and just LF next
		/// </summary>
		public static ReadOnlyCollection<String> SplitIntoLines( String multilineString )
		{
			if( multilineString == null )
			{
				return null;
			}

			return multilineString.Split
			(
				new String[] { "\r\n", "\n" },
				StringSplitOptions.None // Keep empty lines
			)
			.ToList()
			.AsReadOnly();
		}

		/// <summary>
		/// Removes lines from a multilineString that start with the targetCharacter
		/// </summary>
		public static String PruneLines( String multilineString, String startsWith )
		{
			StringBuilder sb = new StringBuilder();
			IList<String> lines = SplitIntoLines( multilineString );
			foreach( String line in lines )
			{
				if( !line.StartsWith( startsWith ) )
				{
					sb.AppendLine( line );
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Returns percent similarity between two files
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public static Percent TextFileChurn( String x, String y )
		{
			if( 0 == String.Compare( x, y ) )
			{
				return Percent.Zero;
			}
			else if( ( String.IsNullOrEmpty( x ) && !String.IsNullOrEmpty( y ) )
				|| ( !String.IsNullOrEmpty( x ) && String.IsNullOrEmpty( y ) ) )
			{
				return Percent.OneHundred;
			}

			IList<String> xlines = SplitIntoLines( x );
			IList<String> ylines = SplitIntoLines( y );

			Int32 onlyInX = 0, onlyInY = 0;

			foreach( String xline in xlines )
			{
				if( !String.IsNullOrWhiteSpace( xline ) && !ylines.Contains( xline ) )
				{
					onlyInX++;
				}
			}

			foreach( String yline in ylines )
			{
				if( !String.IsNullOrWhiteSpace( yline ) && !xlines.Contains( yline ) )
				{
					onlyInY++;
				}
			}

			float score = ( float )( xlines.Count() + ylines.Count() - onlyInX - onlyInY )
				/
				( float )( xlines.Count() + ylines.Count() );

			return new Percent( 1 - score );
		}

		#endregion Text File Helper Methods

		#region Formatting Helper Methods

		// Review these periodically to ensure the Framework doesnt' now offer them natively

		public static String FormatDateMakeShort( DateTime dateTime )
		{
			String retVal = String.Empty;
			if( dateTime != DateTime.MaxValue && dateTime.Year != 9999 && dateTime.Year != 1900 )
			{
				retVal = String.Format( "{0}/{1}/{2}", dateTime.Month, dateTime.Day, dateTime.Year );
			}
			return retVal;
		}

		public static String FormatDateMakeShort( DateTime? dateTime )
		{
			String retVal = String.Empty;

			if( dateTime.HasValue ) retVal = FormatDateMakeShort( dateTime.Value );

			return retVal;
		}

		public static String FormatDateMakeShort( String dateTime )
		{
			String retVal = String.Empty;
			if( !String.IsNullOrEmpty( dateTime ) && !dateTime.StartsWith( "12/31/9999" ) && !dateTime.StartsWith( "1/1/1900" ) )
			{
				DateTime formattedDate = DateTime.Parse( dateTime );
				retVal = String.Format( "{0}/{1}/{2}", formattedDate.Month, formattedDate.Day, formattedDate.Year );
			}
			return retVal;
		}

		public static String FormatDateMakeLong( DateTime? dateTime )
		{
			String retVal = String.Empty;
			if( dateTime.HasValue )
			{
				retVal = dateTime.ToString();
			}
			return retVal;
		}

		public static String FormatDateMakeLong( String dateTime )
		{
			String retVal = String.Empty;

			if( !String.IsNullOrEmpty( dateTime ) && !dateTime.StartsWith( "12/31/9999" ) )
			{
				DateTime formattedDate = DateTime.Parse( dateTime );
				retVal = formattedDate.ToString();
			}

			return retVal;
		}

		/// <summary>
		/// Replaces all instances of URIs (e.g. "http://foo.com") in the text blob with links
		/// (e.g., " <a href="http://foo.com" target="blank">http://foo.com</a>"
		/// </summary>
		public static String LinkifyURLs( String textWithLinks )
		{
			String output = textWithLinks;

			if( !String.IsNullOrWhiteSpace( output ) )
			{
				MatchCollection mactches = CommonRegex.UrlRegex.Matches( output );

				foreach( Match match in mactches )
				{
					output = output.Replace
					(
						match.Value,
						String.Format( "<a href='{0}' target='blank'>{0}</a>", match.Value )
					);
				}
			}

			return output;
		}

		/// <summary>
		/// Removes any
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static String StripWhitespace( String input )
		{
			return CommonRegex.WhiteSpaceRegex.Replace( input, String.Empty );
		}

		#endregion

		#region DbIsNull Helper Methods

		/// <summary>
		/// Returns the Guid if a guid can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static object DbIsNullGuid( object guid )
		{
			object ret = System.DBNull.Value;
			Guid tempGuid = Guid.Empty;

			if( guid != null )
			{
				if( guid is Guid )
				{
					ret = ( Guid )guid;
				}
				else
				{
					String stringGuid = guid.ToString();
					if( !String.IsNullOrWhiteSpace( stringGuid )
						&& Guid.TryParse( stringGuid, out tempGuid ) )
					{
						ret = tempGuid;
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// Returns the Guid if a guid can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static object DbIsNullByte( object val )
		{
			object ret = System.DBNull.Value;
			Byte temp = Byte.MinValue;

			if( val != null )
			{
				if( val is Byte )
				{
					ret = ( Byte )val;
				}
				else
				{
					String stringByte = val.ToString();
					if( !String.IsNullOrWhiteSpace( stringByte )
						&& Byte.TryParse( stringByte, out temp ) )
					{
						ret = temp;
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// Returns the String if it is defined, System.DBNull.Value otherwise
		/// </summary>
		/// <param name="intVal"></param>
		/// <returns></returns>
		public static object DbIsNullString( String val )
		{
			if( val != null )
			{
				return val;
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the String if it is defined, System.DBNull.Value otherwise
		/// </summary>
		/// <param name="intVal"></param>
		/// <returns></returns>
		public static object DbIsNullString( object stringVal, Boolean nullIfEmpty = false )
		{
			if( stringVal != null && stringVal.GetType() == typeof( String ) )
			{
				if( nullIfEmpty && stringVal.ToString() == String.Empty )
				{
					return System.DBNull.Value;
				}
				else
				{
					return stringVal;
				}
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the String if it is defined, System.DBNull.Value otherwise
		/// </summary>
		/// <param name="intVal"></param>
		/// <returns></returns>
		public static object DbIsNullDateTime( DateTime? dateTime )
		{
			if( dateTime.HasValue )
			{
				return dateTime;
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the String if it is defined, System.DBNull.Value otherwise
		/// </summary>
		/// <param name="intVal"></param>
		/// <returns></returns>
		public static object DbIsNullDateTime( object dateTimeVal )
		{
			DateTime temp = DateTime.MinValue;

			if( dateTimeVal != null )
			{
				if( dateTimeVal is DateTime )
				{
					return dateTimeVal;
				}
				else if( DateTime.TryParse( dateTimeVal.ToString(), out temp ) )
				{
					return temp;
				}
				else
				{
					return System.DBNull.Value;
				}
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Int16 if it is defined, System.DBNull.Value otherwise
		/// </summary>
		public static object DbIsNullInt16( Nullable<Int16> int16Val )
		{
			if( int16Val.HasValue )
			{
				return int16Val.Value;
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Int16 if it is defined, System.DBNull.Value otherwise
		/// </summary>
		public static object DbIsNullInt16( object intVal )
		{
			Int16 temp = -1337;

			if( intVal != null )
			{
				if( intVal is Int16 )
				{
					return ( Int16 )intVal;
				}
				else if( Int16.TryParse( intVal.ToString(), out temp ) )
				{
					return temp;
				}
				else
				{
					return System.DBNull.Value;
				}
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Int32 if it is defined, System.DBNull.Value otherwise
		/// </summary>
		public static object DbIsNullInt32( Int32? intVal )
		{
			if( intVal.HasValue )
			{
				return intVal.Value;
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Int32 if it is defined, System.DBNull.Value otherwise
		/// </summary>
		public static object DbIsNullInt32( object intVal )
		{
			Int32 temp = -1337;

			if( intVal != null )
			{
				if( intVal is Int32 )
				{
					return ( Int32 )intVal;
				}
				else if( Int32.TryParse( intVal.ToString(), out temp ) )
				{
					return temp;
				}
				else
				{
					return System.DBNull.Value;
				}
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Int32 if it is defined, System.DBNull.Value otherwise
		/// </summary>
		public static object DbIsNullDouble( double? doubleVal )
		{
			if( doubleVal.HasValue )
			{
				return doubleVal.Value;
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Int32 if it is defined, System.DBNull.Value otherwise
		/// </summary>
		public static object DbIsNullDouble( object doubleVal )
		{
			Double temp = Double.MinValue;

			if( doubleVal != null )
			{
				if( doubleVal is Double )
				{
					return ( Double )doubleVal;
				}
				else if( Double.TryParse( doubleVal.ToString(), out temp ) )
				{
					return temp;
				}
				else
				{
					return System.DBNull.Value;
				}
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the Boolean if it is defined, System.DBNull.Value otherwise
		/// </summary>
		/// <param name="intVal"></param>
		/// <returns></returns>
		public static object DbIsNullBoolean( Boolean? boolVal )
		{
			if( boolVal.HasValue )
			{
				return boolVal.Value;
			}
			else
			{
				return System.DBNull.Value;
			}
		}

		/// <summary>
		/// Returns the argument if it's nonnull and nonempty, System.DBNull.Value otherwise
		/// </summary>
		/// <param name="foo"></param>
		/// <returns></returns>
		public static object DbNullIfEmpty( String foo )
		{
			if( String.IsNullOrEmpty( foo ) )
			{
				return System.DBNull.Value;
			}
			else
			{
				return foo;
			}
		}

		public static Type GetClrType( SqlDbType sqlType )
		{
			switch( sqlType )
			{
				case SqlDbType.BigInt:
					return typeof( Int64? );

				case SqlDbType.Binary:
				case SqlDbType.Image:
				case SqlDbType.Timestamp:
				case SqlDbType.VarBinary:
					return typeof( Byte[] );

				case SqlDbType.Bit:
					return typeof( Boolean? );

				case SqlDbType.Char:
				case SqlDbType.NChar:
				case SqlDbType.NText:
				case SqlDbType.NVarChar:
				case SqlDbType.Text:
				case SqlDbType.VarChar:
				case SqlDbType.Xml:
					return typeof( String );

				case SqlDbType.DateTime:
				case SqlDbType.SmallDateTime:
				case SqlDbType.Date:
				case SqlDbType.Time:
				case SqlDbType.DateTime2:
					return typeof( DateTime? );

				case SqlDbType.Decimal:
				case SqlDbType.Money:
				case SqlDbType.SmallMoney:
					return typeof( decimal? );

				case SqlDbType.Float:
					return typeof( double? );

				case SqlDbType.Int:
					return typeof( Int32? );

				case SqlDbType.Real:
					return typeof( float? );

				case SqlDbType.UniqueIdentifier:
					return typeof( Guid? );

				case SqlDbType.SmallInt:
					return typeof( Int16? );

				case SqlDbType.TinyInt:
					return typeof( Byte? );

				case SqlDbType.Variant:
				case SqlDbType.Udt:
					return typeof( object );

				case SqlDbType.Structured:
					return typeof( DataTable );

				case SqlDbType.DateTimeOffset:
					return typeof( DateTimeOffset? );

				default:
					throw new ArgumentOutOfRangeException( "sqlType" );
			}
		}

		#endregion DbIsNull Helper Methods

		#region General Helper Methods

		public static double ConvertMillisecondsToSeconds( Int64 milliseconds )
		{
			return ( milliseconds / 1000f );
		}

		public static double ConvertBytesToMegabytes( Int64 bytes )
		{
			return ( bytes / 1024f ) / 1024f;
		}

		public static String GetAlias( String domainAndAlias )
		{
			if( String.IsNullOrEmpty( domainAndAlias ) )
			{
				return domainAndAlias;
			}

			if( domainAndAlias.Contains( "/" ) )
			{
				return domainAndAlias.Split( new char[] { '/' } )[ 1 ].ToLower();
			}
			else if( domainAndAlias.Contains( "\\" ) )
			{
				return domainAndAlias.Split( new char[] { '\\' } )[ 1 ].ToLower();
			}
			else return domainAndAlias;
		}

		public static DateTime TimeDateStampFromEpoch( UInt32 epoch )
		{
			DateTime dt = new DateTime( 1970, 1, 1, 0, 0, 0 );
			dt = dt.AddSeconds( Convert.ToDouble( epoch ) ).ToLocalTime();
			return dt;
		}

		public static UInt32 EpochFromTimeDateStamp( DateTime date )
		{
			DateTime origin = new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc );
			TimeSpan diff = date - origin;
			return ( UInt32 )Math.Floor( diff.TotalSeconds );
		}

		/// <summary>
		/// Same as String.EndsWith except it takes a List instead of a single String
		/// </summary>
		public static Boolean EndsWithAny( String stringToTest, IEnumerable<String> endings )
		{
			Boolean retVal = false;

			foreach( String ending in endings )
			{
				if( stringToTest.EndsWith( ending ) )
				{
					retVal = true;
					break;
				}
			}

			return retVal;
		}

		/// <summary>
		/// Returns a list of entries of type T where T is System.Enum
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<T> EnumToList<T>()
		{
			Type enumType = typeof( T );

			// Can't use type constraints on value types, so have to do check like this
			if( enumType.BaseType != typeof( Enum ) )
			{
				throw new ArgumentException( "T must be of type System.Enum" );
			}

			Array enumValArray = Enum.GetValues( enumType );

			List<T> enumValList = new List<T>( enumValArray.Length );

			foreach( var val in enumValArray )
			{
				enumValList.Add( ( T )Enum.Parse( enumType, val.ToString() ) );
			}

			return enumValList;
		}

		/// <summary>
		/// Returns a list of entries of type T where T is System.Enum and Nullable
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static List<Nullable<T>> NullableEnumToList<T>() where T : struct
		{
			Type enumType = typeof( T );

			// Can't use type constraints on value types, so have to do check like this
			if( enumType.BaseType != typeof( Enum ) )
			{
				throw new ArgumentException( "T must be of type System.Enum" );
			}

			Array enumValArray = Enum.GetValues( enumType );

			List<Nullable<T>> enumValList = new List<Nullable<T>>( enumValArray.Length );

			// Add the null value
			enumValList.Add( default( T? ) );

			foreach( var val in enumValArray )
			{
				enumValList.Add( ( T )Enum.Parse( enumType, val.ToString() ) );
			}

			return enumValList;
		}

		/// <summary>
		/// If input is not null or empty, returns a System.String with leading and trailing spaces
		/// trimmed. Otherwise returns the specified default String.
		/// </summary>
		public static String ValueOrString( object foo, String defaultString )
		{
			if( foo != null )
			{
				String tmp = Convert.ToString( foo ).Trim();
				if( tmp.Length <= 0 )
					return defaultString;
				else
					return tmp;
			}
			else
			{
				return defaultString;
			}
		}

		/// <summary>
		/// Converts "1,2,3" to Int32[] { 1, 2, 3 }
		/// </summary>
		public static Int32[] ConvertIntegerListString( String integerListString )
		{
			if( String.IsNullOrEmpty( integerListString ) )
				return null;

			String[] integerStrings = integerListString.Split( new char[] { ',' } );
			List<Int32> integerArray = new List<Int32>( integerStrings.GetLength( 0 ) );
			foreach( String integerString in integerStrings )
			{
				// Which column?
				Int32 integer = Int32.Parse( integerString );
				integerArray.Add( integer );
			}
			return integerArray.ToArray();
		}

		public static Boolean IsObjectEmpty( object foo )
		{
			Boolean isEmpty = false;
			if( foo == null )
				isEmpty = true;
			else
				isEmpty = String.IsNullOrEmpty( foo.ToString() );

			return isEmpty;
		}

		public static Boolean ConvertIntToBool( Int32 dataVal )
		{
			if( dataVal == 0 )
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		public static Boolean ConvertObjectToBool( object dataVal )
		{
			Boolean bRetVal = false;
			try
			{
				String sTmp = Common.ValueOrString( dataVal, "" ).ToLower();
				if( sTmp.Length > 0 )
				{
					if( !Boolean.TryParse( sTmp, out bRetVal ) )
					{
						Int32 iTmp = 0;
						if( Int32.TryParse( sTmp, out iTmp ) )
						{
							if( iTmp != 0 )
								bRetVal = true;
						}
						else
						{
							if( sTmp == "true" )
								bRetVal = true;
						}
					}
				}
			}
			catch( System.Exception )
			{
				bRetVal = false;
			}
			return bRetVal;
		}

		public static TriBool ConvertObjectToTriBool( System.Nullable<Boolean> dataVal )
		{
			TriBool retVal = TriBool.Null;
			if( dataVal == true )
				retVal = TriBool.True;
			else if( dataVal == false )
				retVal = TriBool.False;
			return retVal;
		}

		public static TriBool ConvertObjectToTriBool( object dataRowViewObject )
		{
			TriBool retVal = TriBool.Null;

			if( dataRowViewObject == System.DBNull.Value )
				retVal = TriBool.Null;
			else if( ( Boolean )dataRowViewObject == true )
				retVal = TriBool.True;
			else if( ( Boolean )dataRowViewObject == false )
				retVal = TriBool.False;

			return retVal;
		}

		public static object ConvertTriBoolToObject( TriBool triBoolObject )
		{
			object retVal = System.DBNull.Value;

			switch( triBoolObject )
			{
				case ( TriBool.Null ):
					// already null;
					break;

				case ( TriBool.True ):
					retVal = System.Data.SqlTypes.SqlBoolean.True;
					break;

				case ( TriBool.False ):
					retVal = System.Data.SqlTypes.SqlBoolean.False;
					break;
			}

			return retVal;
		}

		/// Error-Safe integer conversion. Returns foo converted to integer. If foo is null, or
		/// cannot be converted to Int32, then the specified default value is returned. </summary>
		public static Int32 ValueOrInt( object foo, Int32 defaultValue )
		{
			Int32 iRetVal = defaultValue;
			if( foo != null )
			{
				try
				{
					iRetVal = Convert.ToInt32( foo );
				}
				catch
				{
					iRetVal = defaultValue;
				}
			}
			return iRetVal;
		}

		public static String NullIfEmpty( String foo )
		{
			if( String.IsNullOrEmpty( foo ) )
			{
				return null;
			}
			else
			{
				return foo;
			}
		}

		/// <summary>
		/// Ensures corrent format, e.g., /A/B/C -&gt; a\b\c\
		/// </summary>
		/// <param name="directoryNode"></param>
		/// <returns></returns>
		public static String ScrubDirectoryNode( String directoryNode )
		{
			if( String.IsNullOrEmpty( directoryNode ) )
			{
				throw new ArgumentNullException( nameof( directoryNode ) );
			}

			directoryNode = directoryNode.Trim().ToLower().Replace( '/', '\\' );

			// Ensure no leading slash
			if( directoryNode.StartsWith( @"\" ) )
			{
				directoryNode = directoryNode.Remove( 0, 1 );
			}
			// Ensure terminal slash
			if( !directoryNode.EndsWith( @"\" ) )
			{
				directoryNode = directoryNode + @"\";
			}
			return directoryNode;
		}

		#endregion General Helper Methods

		#region IsNull Converters

		public static Boolean IsNullBool( object foo, Boolean val )
		{
			if( foo == null )
			{
				return val;
			}

			Boolean result;

			if( Boolean.TryParse( foo.ToString(), out result ) )
			{
				return result;
			}
			else
			{
				return val;
			}
		}

		/// <summary>
		/// Returns the argument cast as String if nonnull, null otherwise
		/// </summary>
		public static String IsNullString( object foo )
		{
			if( foo != null )
			{
				return Convert.ToString( foo );
			}
			else return null;
		}

		public static DateTime? IsNullDateTime( object foo )
		{
			DateTime? ret = null;

			if( foo != null && foo is DateTime )
			{
				ret = ( DateTime )foo;
			}

			return ret;
		}

		/// <summary>
		/// Returns the Guid if a guid can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static Nullable<Guid> IsNullGuid( object guid )
		{
			Nullable<Guid> ret = null;
			Guid tempGuid = Guid.Empty;

			if( guid != null )
			{
				if( guid is Guid )
				{
					ret = ( Guid )guid;
				}
				else
				{
					String stringGuid = guid.ToString();
					if( !String.IsNullOrWhiteSpace( stringGuid )
						&& Guid.TryParse( stringGuid, out tempGuid ) )
					{
						ret = tempGuid;
					}
				}
			}

			return ret;
		}

		public static Int32? IsNullInt32( XmlAttribute foo )
		{
			Int32 x;
			if( foo != null && Int32.TryParse( foo.Value, out x ) )
			{
				return x;
			}
			else return null;
		}

		/// <summary>
		/// Returns the integer if an integer can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static Int32? IsNullInt32( object foo )
		{
			Int32? ret = null;
			Int32 tempInt = Int32.MinValue;

			if( foo != null && Int32.TryParse( foo.ToString(), out tempInt ) )
			{
				ret = tempInt;
			}

			return ret;
		}

		/// <summary>
		/// Returns the float if an float can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static double? IsNullDouble( object foo )
		{
			double? ret = null;
			double temp = double.MinValue;

			if( foo != null && double.TryParse( foo.ToString(), out temp ) )
			{
				ret = temp;
			}

			return ret;
		}

		/// <summary>
		/// Returns the boolean if an boolean can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static Boolean? IsNullBoolean( object foo )
		{
			Boolean? ret = null;
			Boolean temp = false;

			if( foo != null && Boolean.TryParse( foo.ToString(), out temp ) )
			{
				ret = temp;
			}

			return ret;
		}

		/// <summary>
		/// Returns the boolean if an boolean can be extracted/parsed from the object, null otherwise
		/// </summary>
		public static Byte? IsNullByte( object foo )
		{
			Byte? ret = null;
			Byte temp = 0;

			if( foo != null && Byte.TryParse( foo.ToString(), out temp ) )
			{
				ret = temp;
			}

			return ret;
		}

		#endregion IsNull Converters

		#region Class and Object Methods

		public static object GetDefault( Type type )
		{
			if( type.IsValueType )
			{
				return Activator.CreateInstance( type );
			}
			return null;
		}

		/// <summary>
		/// Constructs a derived class instance from a base class instance
		/// </summary>
		/// <typeparam name="TBase">The base class instance type</typeparam>
		/// <typeparam name="TDerived">The requested derived class instance type</typeparam>
		/// <param name="baseInstance">The instance of the base class</param>
		/// <returns>A derived class instance</returns>
		public static TDerived Construct<TBase, TDerived>( TBase baseInstance )
			where TDerived : TBase, new()
		{
			TDerived derived = new TDerived();

			PropertyInfo[] baseProps = typeof( TBase ).GetProperties();

			foreach( PropertyInfo bp in baseProps )
			{
				// get derived matching property
				PropertyInfo dp = typeof( TDerived ).GetProperty( bp.Name, bp.PropertyType );

				// this property must not be index property
				if
				(
					dp != null
					&& ( dp.GetSetMethod( true ) != null )
					&& ( bp.GetIndexParameters().Length == 0 )
					&& ( dp.GetIndexParameters().Length == 0 ) )
				{
					dp.SetValue( derived, dp.GetValue( baseInstance, null ), null );
				}
			}

			return derived;
		}



		#endregion Class and Object Methods

		#region Comparison Helper Methods

		/// <summary>
		/// Compares two strings character by character, returns 0 if they're identical, or else the
		/// character at which they diverge
		/// </summary>
		/// <returns></returns>
		public static Int32 Compare( String x, String y )
		{
			Int32 xlen = x.Length;
			Int32 ylen = y.Length;
			Int32 commonLen = Math.Min( xlen, ylen );
			Int32 index = 0;

			for( Int32 k = 0; k < Math.Min( xlen, ylen ); k++ )
			{
				if( x[ k ] != y[ k ] )
				{
					index = k;
					break;
				}
			}

			if( index == 0 && xlen != ylen )
			{
				index = commonLen;
			}

			return index;
		}

		#endregion Comparison Helper Methods

		#region XML Helper functions

		public static XmlDocument ToXmlDocument( XDocument xDocument )
		{
			var xmlDocument = new XmlDocument();
			using( var xmlReader = xDocument.CreateReader() )
			{
				xmlDocument.Load( xmlReader );
			}
			return xmlDocument;
		}

		/// <summary>
		/// Checks the XmlAttribute for null before gettings its value
		/// </summary>
		/// <returns>The String value of the XmlAttribute, or null if the XmlAttribute is null</returns>
		public static String IsNullXmlAttribute( XmlAttribute foo )
		{
			if( foo != null )
			{
				return foo.Value;
			}
			else return null;
		}

		/// <summary>
		/// Checks the XAttribute for null before gettings its value
		/// </summary>
		/// <returns>The String value of the XAttribute, or null if the XAttribute is null</returns>
		public static String IsNullXAttribute( XAttribute foo )
		{
			if( foo != null )
			{
				return foo.Value;
			}
			else return null;
		}

		/// <summary>
		/// Checks the XElement for null before gettings its value
		/// </summary>
		/// <returns>The String value of the XElement, or null if the XElement is null</returns>
		public static String IsNullXElement( XElement foo )
		{
			if( foo != null )
			{
				return foo.Value.Trim();
			}
			else return null;
		}

		#endregion XML Helper functions

		#region Special Hashes

		/// <summary>
		/// Combines the HashCodes of a set of objects as a simple test of set equality
		/// </summary>
		public static Int32 GetScarabHashCodeOfList<T>( IList<T> objs )
		{
			Debug.WriteLine( "--> CommonEx.GetScarabHashCodeOfList()" );

			if( objs == null || objs.Count == 0 )
			{
				return 0;
			}

			List<Int32> hashCodes = objs.Select( o => o.GetHashCode() ).ToList();
			String cdl = Common.CommaDelimitedList<Int32>( hashCodes, false );
			Debug.WriteLine( "--> {0} items: {1}", hashCodes.Count, cdl );

			return Hash.HashString32( cdl );
		}

		/// <summary>
		/// Hashes the concatenation of property values as a simple test of instance equality
		/// </summary>
		public static Int32 GetScarabHashCode<T>( T instance, Boolean ignoreUnderscoreProps = true )
		{
			Debug.Assert( instance != null );
			StringBuilder sb = new StringBuilder();
			PropertyInfo[] props = typeof( T ).GetProperties();

			foreach( PropertyInfo pi in props )
			{
				if( ignoreUnderscoreProps && pi.Name.StartsWith( "_" ) )
				{
					continue;
				}

				String propVal = pi.GetValue( instance, null ) == null
					? "EMPTY"
					: pi.GetValue( instance, null ).ToString();

				sb.Append( propVal );

				Debug.WriteLine( "{0}.{1}={2}", typeof( T ).Name, pi.Name, propVal );
			}

			Int32 hash = Hash.HashString32( sb.ToString() );

			return hash;
		}

		#endregion Special Hashes

		#region Spare Code

		//public static String[] ConvertToStringArray( StringCollection stringCollection )
		//{
		//	String[] stringArray = null; // Preserve null
		//	if( stringCollection != null )
		//	{
		//		stringArray = new String[ stringCollection.Count ];
		//		stringCollection.CopyTo( stringArray, 0 );
		//	}
		//	return stringArray;
		//}

		#endregion Spare Code
	}
}