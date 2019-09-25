using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tamasi.Shared.Framework.Types
{
	/// <summary>
	/// Thread-safe looping integer index
	/// </summary>
	public class LoopIndex
	{
		private Int32 currentIndex = 0;

		public LoopIndex( Int32 size )
		{
			this.Size = size;
			this.Sync = new object();
		}

		public Int32 Size { get; private set; }

		public object Sync { get; private set; }

		public Int32 CurrentIndex
		{
			get
			{
				lock( this.Sync )
				{
					return currentIndex;
				}
			}
		}

		public Int32 NextIndex()
		{
			lock( this.Sync )
			{
				if( currentIndex < this.Size - 1 )
				{
					Interlocked.Increment( ref currentIndex );
				}
				else
				{
					currentIndex = 0;
				}

				return currentIndex;
			}
		}
	}
}
