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

namespace Tamasi.Shared.Framework.Comparers
{
	/// <summary>
	/// Class that compares two instances of the same class T
	/// </summary>
	public sealed class InstanceComparer
	{
		/// <summary>
		/// Compares property values of two instances of the same class
		/// </summary>
		/// <param name="diffs">The list of diffs</param>
		/// <returns>TRUE if instances are identical, false otherwise</returns>
		public static Boolean CompareObjectInstances<T>
		(
			T instance1,
			T instance2,
			out IList<PropDiff> diffs,
			Boolean ignoreUnderscoreProps = true )
		{
			diffs = new List<PropDiff>();
			Boolean areSame = true;
			PropertyInfo[] props = typeof( T ).GetProperties();

			foreach( PropertyInfo pi in props )
			{
				if( ignoreUnderscoreProps && pi.Name.StartsWith( "_" ) )
				{
					continue;
				}

				object instance1Val = pi.GetValue( instance1, null );
				object instance2Val = pi.GetValue( instance2, null );

				Boolean instance1IsNull = ( instance1Val == null );
				Boolean instance2IsNull = ( instance2Val == null );

				String instance1String = instance1IsNull ? "!NULL!" : instance1Val.ToString();
				String instance2String = instance2IsNull ? "!NULL!" : instance2Val.ToString();

				if( instance1String == String.Empty ) instance1String = "!EMPTY!";
				if( instance2String == String.Empty ) instance2String = "!EMPTY!";

				if( instance1String != instance2String )
				{
					areSame = false;
					diffs.Add( new PropDiff( pi.Name, instance1String, instance2String ) );
				}

				return areSame;
			}

			return true;
		}

		public class PropDiff
		{
			public PropDiff( String propertyName, String instanceVal1, String instanceVal2 )
			{
				this.PropertyName = propertyName;
				this.Instance1Val = instanceVal1;
				this.Instance2Val = instanceVal2;
			}

			public String PropertyName { get; private set; }

			public String Instance1Val { get; private set; }

			public String Instance2Val { get; private set; }

			public static void PrintDiffList( IList<PropDiff> list )
			{
				foreach( PropDiff pd in list )
				{
					Common.WriteLine
					(
						"Prop {0}: '{1}' '{2}'",
						pd.PropertyName,
						pd.Instance1Val,
						pd.Instance2Val
					);
				}
			}
		}
	}
}
