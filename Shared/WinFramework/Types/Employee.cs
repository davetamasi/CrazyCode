using System;
using Tamasi.Shared.Framework;

namespace Tamasi.Shared.WinFramework.Types
{
	/// <summary>
	/// The canonical form of a CompCentral/TFS area path -- \TeamProject\Node1\Node2\Node3\
	/// </summary>
	public sealed class Employee
	{
		#region Fields and Constructors

		public Employee( Alias alias )
		{
			this.Alias = alias;
			this.IsComplete = false;
		}

		public Employee( String fullName )
		{
			this.FullName = fullName;
			this.IsComplete = false;
		}

		public Employee( Alias alias, String fullName )
		{
			this.Alias = alias;
			this.FullName = fullName;
			this.IsComplete = true;
		}

		#endregion

		#region Properties

		public Alias Alias { get; private set; }

		public String FullName { get; private set; }

		public Boolean IsComplete { get; private set; }

		#endregion

		#region Statics and Overrides

		public static Employee Parse
		(
			String employeeString,
			ValidationFailureAction onFailure = ValidationFailureAction.Ignore )
		{
			Employee ret = null;

			if( !String.IsNullOrWhiteSpace( employeeString ) )
			{
				if( CommonRegex.EmployeeAliasRegex.IsMatch( employeeString ) )
				{
					ret = new Employee( Alias.Parse( employeeString ) );
				}
				else if( CommonRegex.EmployeeStringRegex.IsMatch( employeeString ) )
				{
					// Dave Tamasi (DTAMASI)
					String[] split = employeeString.Split( new char[] {'('} );
					Alias alias = Alias.Parse( split[1].Replace( ")", String.Empty ));
					String fullName = split[0].Trim();
					ret = new Employee( alias, fullName );
				}
				else if( CommonRegex.EmployeeNameRegex.IsMatch( employeeString ) )
				{
					ret = new Employee( employeeString );
				}
			}

			return ret;
		}

		public override string ToString()
		{
			String ret = null;

			if( this.IsComplete )
			{
				ret = String.Format( "{0} ({1})", this.FullName, this.Alias );
			}
			else if( this.Alias != null )
			{
				ret = this.Alias;
			}
			else
			{
				ret = this.FullName;
			}

			return ret;
		}

		public static implicit operator string( Employee employee )
		{
			return employee.ToString();
		}

		public static implicit operator Employee( String employeeString )
		{
			return Employee.Parse( employeeString );
		}

		public override Boolean Equals( object obj )
		{
			// If parameter is null return false.
			if( obj == null )
			{
				return false;
			}

			Employee other = obj as Employee;
			if( ( System.Object )other == null )
			{
				return false;
			}

			// Return true if the fields match (may be referenced by derived classes)
			return 0 == string.Compare( this.ToString(), other.ToString(), true );
		}

		public override Int32 GetHashCode()
		{
			return Hash.HashString32( this.ToString() );
		}

		#endregion
	}
}