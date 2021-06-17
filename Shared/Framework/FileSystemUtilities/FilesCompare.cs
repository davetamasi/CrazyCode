using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data.Linq;
using System.Threading;
using System.Threading.Tasks;

using fr = Tamasi.Shared.Framework;

namespace Tamasi.Shared.Framework.FileSystemUtilities
{
	public sealed class FilesCompare
	{
		#region Privates and Constructors

		private readonly Lazy<List<FileInfo>> sourceFiles;
		private readonly Lazy<List<FileInfo>> targetFiles;

		public FilesCompare
		(
			DirectoryInfo sourceDir,
			DirectoryInfo targetDir,
			SearchOption searchOption = SearchOption.TopDirectoryOnly )
		{
			this.sourceFiles = new Lazy<List<FileInfo>>( () =>
			{
				return sourceDir.GetFiles( "*", searchOption ).ToList();
			} );

			this.targetFiles = new Lazy<List<FileInfo>>( () =>
			{
				return targetDir.GetFiles( "*", searchOption ).ToList();
			} );
		}

		#endregion

		#region Properties

		public IList<FileInfo> OnlyInSource
		{
			get { return this.sourceFiles.Value.Except( this.targetFiles.Value ).ToList(); }
		}

		public IList<FileInfo> SameInBoth
		{
			get { return this.sourceFiles.Value.Intersect( this.targetFiles.Value ).ToList(); }
		}

		public IList<FileInfo> NewerInTarget
		{
			get
			{
				var x = from s in this.sourceFiles.Value
						join t in this.targetFiles.Value on s.Name equals t.Name
						where t.LastWriteTime > s.LastWriteTime
						select t;

				return x.ToList();
			}
		}

		public IList<FileInfo> NewerInSource
		{
			get
			{
				var x = from s in this.sourceFiles.Value
						join t in this.targetFiles.Value on s.Name equals t.Name
						where s.LastWriteTime > t.LastWriteTime
						select s;

				return x.ToList();
			}
		}

		public IList<FileInfo> OnlyInTarget
		{
			get { return this.targetFiles.Value.Except( this.sourceFiles.Value ).ToList(); }

		}

		#endregion
	}
}