using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace Tamasi.Shared.Framework
{
	public sealed class SetComparer<TLive, TLocal>
	{
		#region Fields and Constructors

		private IList<object> localSetKeys = null;
		private IList<object> liveSetKeys = null;
		private ConcurrentBag<TLive> inBothButDiff = null;
		private ConcurrentBag<TLive> inBothAndSame = null;

		#endregion

		#region Properties

		/// <summary>
		/// The master set of items
		/// </summary>
		public IList<TLive> LiveSet { get; set; }

		/// <summary>
		/// Field name of the key of the live set
		/// </summary>
		public string LiveSetKey { get; set; }

		/// <summary>
		/// The cached set of items
		/// </summary>
		public IList<TLocal> LocalSet { get; set; }

		/// <summary>
		/// Field name of the key of the cache set
		/// </summary>
		public string LocalSetKey { get; set; }

		#endregion

		#region Public Methods

		public delegate Boolean AreEquivalent( TLive liveRecord, TLocal localRecord );

		/// <summary>
		/// Points to a method that returns Boolean and takes one T record and one K record
		/// </summary>
		public AreEquivalent EquivalenceCallback { get; set; }

		/* EquivalenceCallback example:  set this property to "AreEquivalent" and implement this:
		public static Boolean AreEquivalent( LiveCCRecord liveNode, CompCentralNode localNode )
		{
			Boolean areSame = 0 == string.Compare( liveNode.AreaPath.Replace( @"\", string.Empty ), localNode.AreaPath.Replace( @"\", string.Empty ) )
				&& liveNode.ParentCCNodeID == localNode.CCParentNodeID
				&& liveNode.NodeLevel == localNode.NodeLevel;

			return areSame;
		}
		There are a few built-in below */

		public ReadOnlyCollection<TLive> OnlyInLiveSet()
		{
			ReadOnlyCollection<TLive> onlyInLive = null;

			if( typeof( TLive ).IsPrimitive || typeof( TLive ).GetType() == typeof( string ) )
			{
				var diffKeys = this.LiveSetKeys.Except( this.LocalSetKeys ).ToList();

				onlyInLive = null;
			}
			else if( LiveSet.Count == 0 )
			{
				onlyInLive = new List<TLive>().AsReadOnly();
			}
			else
			{
				FieldInfo liveKeyField = LiveSet[ 0 ].GetType().GetField( this.LiveSetKey );
				//PropertyInfo localKeyField = LocalSet[ 0 ].GetType().GetProperty( LocalSetKey );

				//Type liveKeyType = liveKeyField.GetType();
				//Type localKeyType = localKeyField.GetType();

				////Debug.Assert( liveKeyType == localKeyType );

				//var liveSetKeys = LiveSet.Select( k => liveKeyField.GetValue( k ) ).ToList();
				//var localSetKeys = LocalSet.Select( k => localKeyField.GetValue( k ) ).ToList();

				var diffKeys = this.LiveSetKeys.Except( this.LocalSetKeys ).ToList();

				onlyInLive = LiveSet
					.Where( k => diffKeys.Contains( liveKeyField.GetValue( k ) ) )
					.ToList()
					.AsReadOnly();
			}

			return onlyInLive;
		}

		public ReadOnlyCollection<TLocal> OnlyInLocalSet()
		{
			//FieldInfo liveKeyField = LiveSet[ 0 ].GetType().GetField( LiveSetKey );
			PropertyInfo localKeyField = LocalSet[ 0 ].GetType().GetProperty( this.LocalSetKey );

			//Type liveKeyType = liveKeyField.GetType();
			//Type localKeyType = localKeyField.GetType();

			////Debug.Assert( liveKeyType == localKeyType );

			//var liveSetKeys = LiveSet.Select( k => liveKeyField.GetValue( k ) ).ToList();
			//var localSetKeys = LocalSet.Select( k => localKeyField.GetValue( k ) ).ToList();

			var diffKeys = this.LocalSetKeys.Except( this.LiveSetKeys ).ToList();

			ReadOnlyCollection<TLocal> onlyInLocal = LocalSet
				.Where( k => diffKeys.Contains( localKeyField.GetValue( k ) ) )
				.ToList()
				.AsReadOnly();

			return onlyInLocal;
		}

		public ReadOnlyCollection<TLive> InBothButDifferent()
		{
			if( this.inBothButDiff == null )
			{
				this.CompareMatches();
			}

			return this.inBothButDiff.ToList().AsReadOnly();
		}

		public ReadOnlyCollection<TLive> InBothAndEquivalent()
		{
			if( this.inBothAndSame == null )
			{
				this.CompareMatches();
			}

			return this.inBothAndSame.ToList().AsReadOnly();
		}

		#endregion

		#region Private Methods

		private IList<object> LiveSetKeys
		{
			get
			{
				if( liveSetKeys == null )
				{
					// If this is a simple type, e.g., string, rather than a SQLMetal complex type,
					// then the keys are the data to be compared
					if( typeof( TLive ).IsPrimitive || typeof( TLive ).GetType() == typeof( string ) )
					{
						liveSetKeys = this.LiveSet.Select( k => ( object )k ).ToList();
					}
					else
					{
						FieldInfo liveKeyField = this.LiveSet[ 0 ].GetType().GetField( this.LiveSetKey );
						liveSetKeys = LiveSet.Select( k => liveKeyField.GetValue( k ) ).ToList();
					}
				}

				return liveSetKeys;
			}
		}

		private IList<object> LocalSetKeys
		{
			get
			{
				if( localSetKeys == null )
				{
					if( 1 == LocalSet[ 0 ].GetType().GetMembers().Count() )
					{
						localSetKeys = this.LocalSet.Select( k => ( object )k ).ToList();
					}
					else
					{
						PropertyInfo localKeyField = LocalSet[ 0 ].GetType().GetProperty( this.LocalSetKey );
						localSetKeys = LocalSet.Select( k => localKeyField.GetValue( k ) ).ToList();
					}
				}

				return localSetKeys;
			}
		}

		private void CompareMatches()
		{
			FieldInfo liveKeyField = LiveSet[ 0 ].GetType().GetField( LiveSetKey );
			PropertyInfo localKeyField = LocalSet[ 0 ].GetType().GetProperty( LocalSetKey );

			var sameKeys = this.LiveSetKeys.Intersect( this.LocalSetKeys ).ToList();

			List<TLive> liveSet = this.LiveSet
				.Where( k => sameKeys.Contains( liveKeyField.GetValue( k ) ) )
				.ToList();

			List<TLocal> localSet = this.LocalSet
				.Where( k => sameKeys.Contains( localKeyField.GetValue( k ) ) )
				.ToList();

			// TODO Pri 2 rewrite as LINQ
			this.inBothButDiff = new ConcurrentBag<TLive>();
			this.inBothAndSame = new ConcurrentBag<TLive>();

			foreach( var key in sameKeys.AsParallel() )
			{
				TLive liveRecord = this.LiveSet.Single( k => liveKeyField.GetValue( k ).ToString() == key.ToString() );
				TLocal localRecord = this.LocalSet.Single( k => localKeyField.GetValue( k ).ToString() == key.ToString() );

				if( EquivalenceCallback( liveRecord, localRecord ) )
				{
					this.inBothAndSame.Add( liveRecord );
				}
				else
				{
					this.inBothButDiff.Add( liveRecord );
				}
			}
		}

		#endregion

		#region Statics

		public static Boolean AreEquivalentStrings( string liveRecord, string localRecord )
		{
			return 0 == string.Compare( liveRecord, localRecord );
		}

		#endregion
	}
}