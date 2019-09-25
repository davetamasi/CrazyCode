using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tamasi.Shared.Framework.Types
{
	public class FixedSizedQueue<T>
	{
		#region Fields and Constructors
		
		private readonly ConcurrentQueue<T> q;

		public FixedSizedQueue( Int32 capacity )
		{
			Debug.Assert( capacity >= 2 );
			q = new ConcurrentQueue<T>();
			this.Capacity = capacity;
		}

		#endregion

		public Int32 Capacity { get; set; }

		public Int32 Count
		{
			get { return this.q.Count; }
		}

		public T this[ Int32 index ]
		{
			get
			{
				Debug.Assert( index >= 0 && index < this.Capacity );
				return this.q.ElementAt( index );
			}
		}

		public void Enqueue( T obj )
		{
			q.Enqueue( obj );

			lock( this )
			{
				T overflow;
				while( q.Count > this.Capacity && q.TryDequeue( out overflow ) ) ;
			}

			string message = string.Format( "--> FixedSizedQueue.Enqueued({0})", obj );
			Debug.WriteLine( message );
		}
	}
}
