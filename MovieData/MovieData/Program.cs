using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Tamasi.Shared.Framework.Extensions;
using fr = Tamasi.Shared.Framework;

namespace Tamasi.MovieData
{
	internal static class Program
	{
		public enum NameFormat
		{
			Unrated,
			Rated,
			Unknown
		}

		private static Regex isOldFormat = new Regex( @".*\([0-9]{4}\)\.[a-z]{2,3}" );
		private static Regex isNewFormat = new Regex( @".*\([0-9]{4},[0-9]{2}\)\.[a-z]{2,3}" );

		public static NameFormat GetFileNameFormat( string fileName )
		{
			if( isOldFormat.IsMatch( fileName ) )
			{
				return NameFormat.Unrated;
			}
			else if( isNewFormat.IsMatch( fileName ) )
			{
				return NameFormat.Rated;
			}
			else return NameFormat.Unknown;
		}

		private static void ProcessMovieFile( FileInfo movieFile )
		{
			fr.Common.WriteLine( $"Processing movie file '{movieFile.Name}'" );

			String lookupName = movieFile.Name.Replace( movieFile.Extension, String.Empty );

			// movieFile.Name might be 'Star Wars (1976,99).mkv' or 'Star Wars (1976).mkv'
			lookupName = lookupName.GetUntilOrEmpty( "(" ).Trim();

			// Contact IMDB
			fr.Common.WriteLine( $"  Asking IMDB for '{lookupName}'" );
			ImdbMovie movie = ImdbConnection.GetImdbMovie( lookupName );
			float ratingStringTemp = 0;

			if( movie == null || movie.Title == null )
			{
				fr.Common.DrawWarning( $"Unable to locate '{lookupName}' in IMDB" );
			}
			else if( !float.TryParse( movie.ImdbRating, out ratingStringTemp ) )
			{
				fr.Common.DrawWarning( $"Unable to derive rating for '{lookupName}' from IMDB" );
			}
			else
			{
				String ratingString = ( ratingStringTemp * 10 ).ToString();
				String newName = movieFile.Name.Replace( ")", String.Format( ",{0})", ratingString ) );

				switch( GetFileNameFormat( movieFile.Name ) )
				{
					case NameFormat.Rated:
						// TODO: compare ratings and update if different
						Console.Write( "   Already rated" );
						break;

					case NameFormat.Unrated:
						Console.Write( "   Rename to '{0}'?", newName );
						ConsoleKeyInfo cki = Console.ReadKey();

						if( cki.KeyChar.ToString().ToLower() == "y" )
						{
							String newFullMoviePath = Path.Combine( movieFile.DirectoryName, newName );
							File.Move( movieFile.FullName, newFullMoviePath );
							String originalSrtFileName = movieFile.FullName.Replace( movieFile.Extension, ".srt" );

							if( File.Exists( originalSrtFileName ) )
							{
								String newSrtFilePath = newFullMoviePath.Replace( movieFile.Extension, ".srt" );
								File.Move( originalSrtFileName, newSrtFilePath );
							}

							Console.WriteLine( "  Renamed." );
						}
						else
						{
							Console.WriteLine( "  Not renamed." );
						}
						break;

					case NameFormat.Unknown:
						fr.Common.DrawWarning( $"Unknown filename format: '{lookupName}'" );
						break;
				}
			}

			Console.WriteLine();
		}

		private static void Main( string[] args )
		{
			// Connect to the movie folder
			String movieDirPath = ConfigurationManager.AppSettings[ "movieDirPath" ];
			DirectoryInfo movieRootDir = new DirectoryInfo( movieDirPath );
			Console.Write( $"Found movie folder '{movieRootDir.FullName}'" );

			foreach( FileInfo movieFile in movieRootDir.GetFiles( "*", SearchOption.TopDirectoryOnly ) )
			{
				// Only process if format matches a video format
				if( new String[] { ".avi", ".mp4", ".mkv" }.Contains( movieFile.Extension.ToLower() ) )
				{
					ProcessMovieFile( movieFile );
				}
			}

			Console.ReadKey();
		}
	}
}