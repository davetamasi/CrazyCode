using System;

using System.Data.Entity;

namespace Wedding2
{
	/// <summary>
	/// Summary description for DataContext
	/// </summary>
	public class DataContext : DbContext

	{
		public DataContext()
		{
		}

		public DbSet<Guest> Guests { get; set; }
	}

	public class Guest
	{
		public Byte GuestID { get; set; }

		public String PartyName { get; set; }

		public Byte MaxSize { get; }

		public Byte _Count { get; set; }

		public String _Accommodations { get; set; }

		public String _Notes { get; set; }

		public DateTime _RsvpDate { get; set; }
	}
}