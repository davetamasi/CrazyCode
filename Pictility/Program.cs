using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using fr = Tamasi.Shared.Framework;
using Tamasi.Shared.Framework.FileSystemUtilities;

namespace Tamasi.Pictility
{
	/// <summary>
	/// This Picture Utility creates smaller versions of images in the
	/// main image library at a specified location
	/// </summary>
	internal class Program
	{
		static readonly Int32 IMAGE_SIZE = Int32.Parse( ConfigurationManager.AppSettings["imageSize"] );
		static readonly Int64 QUALITY = Int64.Parse( ConfigurationManager.AppSettings["compressionQuality"] );
		static object lockObj = new object();

		private static ImageCodecInfo? GetEncoder( ImageFormat format )
		{
			ImageCodecInfo? ret = null;
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

			foreach( ImageCodecInfo codec in codecs )
			{
				if( codec.FormatID == format.Guid )
				{
					ret = codec;
				}
			}

			return ret;
		}

		public static void CreateSmallCopy( FileInfo sourceImageFile, FileInfo targetImageFile )
		{
			using( Image sourceImage = Image.FromFile( sourceImageFile.FullName ) )
			{
				// Determine how to resize -- we want the longest side to be IMAGE_SIZE
				Single scaleFactor = 1;
				if( sourceImage.Width >= sourceImage.Height )
				{
					// Landscape
					if( sourceImage.Width > IMAGE_SIZE )
					{
						// E.g., if longer edge is 2048, the scaleFactor is 0.5
						scaleFactor = IMAGE_SIZE / ( Single )sourceImage.Width;
					}
				}
				else
				{
					// Portrait
					if( sourceImage.Height > IMAGE_SIZE )
					{
						scaleFactor = IMAGE_SIZE / ( Single )sourceImage.Height;
					}
				}

				Int32 newWidth = ( Int32 )( sourceImage.Width * scaleFactor + 1 );
				Int32 newHeight = ( Int32 )( sourceImage.Height * scaleFactor + 1 );

				// Create an Encoder object based on the GUID 
				// for the Quality parameter category.
				// https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-set-jpeg-compression-level?redirectedfrom=MSDN

				ImageCodecInfo? jgpEncoder = GetEncoder(ImageFormat.Jpeg);
				Encoder myEncoder = Encoder.Quality;
				EncoderParameters myEncoderParameters = new EncoderParameters(1);
				EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, QUALITY);
				myEncoderParameters.Param[ 0 ] = myEncoderParameter;

				using( Bitmap bitmap = new Bitmap( newWidth, newHeight ) )
				using( var graphics = Graphics.FromImage( bitmap ) )
				{
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
					graphics.CompositingQuality = CompositingQuality.HighQuality;

					graphics.DrawImage( sourceImage, new Rectangle( 0, 0, newWidth, newHeight ) );
					bitmap.Save( targetImageFile.FullName, jgpEncoder, myEncoderParameters );
				}

				File.SetLastWriteTime( targetImageFile.FullName, sourceImageFile.LastWriteTime );
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sourceDir"></param>
		private static void ProcessDirectory( DirectoryInfo sourceDir )
		{


		}

		private static void Main( string[] args )
		{
			DirectoryInfo sourceDir = new DirectoryInfo(ConfigurationManager.AppSettings["sourceDirectoryPath"]);
			DirectoryInfo targetDir = new DirectoryInfo(ConfigurationManager.AppSettings["targetDirectoryPath"]);

			fr.Common.WriteLine( "-> Syncing {0} to {1}", sourceDir.FullName, targetDir.FullName );

			// Delete any orphans
			FilesCompare f = new FilesCompare( sourceDir, targetDir );
			foreach( FileInfo fi in f.OnlyInTarget )
			{
				fr.Common.WriteLine( "-> Deleting orphan {0}", fi.FullName );
				File.Delete( fi.FullName );
			}

			Parallel.ForEach<FileInfo>
			(
				sourceDir.EnumerateFiles( "*.jpg", SearchOption.AllDirectories ),
				new ParallelOptions()
				{
					MaxDegreeOfParallelism = 1
				},
				sourcePic =>
				{
					String dirAndFile = sourcePic.FullName.Replace( sourceDir.FullName + "\\", String.Empty );
					FileInfo targetPic = new FileInfo( Path.Combine( targetDir.FullName, dirAndFile ) );

					lock( lockObj )
					{
						if( !targetPic.Directory.Exists )
						{
							fr.Common.WriteLine( "-> Creating directory {0}", targetPic.Directory.FullName );
							Directory.CreateDirectory( targetPic.Directory.FullName );
						}
					}

					if( !targetPic.Exists )
					{
						fr.Common.WriteLine( "-> Creating {0}", targetPic.FullName );
						CreateSmallCopy( sourcePic, targetPic );
					}
					else if( targetPic.Exists && sourcePic.LastWriteTime != targetPic.LastWriteTime )
					{
						fr.Common.WriteLine( "-> Updating {0}", targetPic.FullName );
						CreateSmallCopy( sourcePic, targetPic );
					}
				} );
		}
	}
}