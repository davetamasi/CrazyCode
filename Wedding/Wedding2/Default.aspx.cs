using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using WeddingData;

public partial class _Default : System.Web.UI.Page
{
	protected Guest guest;
	protected Guid guestID;

	protected Boolean IsAttending
	{
		get
		{
			return guest != null && guest._Count.HasValue && guest._Count.Value > 0;
		}
	}

	protected Boolean IsLodging
	{
		get
		{
			return this.guest != null && this.guest.Lodging != null;
		}
	}

	protected void Page_Load( object sender, EventArgs e )
	{
		if( !Page.IsPostBack )
		{
			Session[ "GuestID" ] = null;
		}
		else
		{
			if( Session[ "GuestID" ] != null )
			{
				if( !Guid.TryParse( Session[ "GuestID" ].ToString(), out guestID ) )
				{
					Debug.Fail( "Invalid guid" );
				}
				Debug.Assert( guestID != Guid.Empty );
			}

			if( guestID != Guid.Empty && guest == null )
			{
				using( WeddingContext db = WeddingContext.New() )
				{
					Debug.Assert( db.DatabaseExists() );
					Debug.Assert( guestID != Guid.Empty );

					// Find person from db
					var query = from b in db.Guest
								where b.GuestID == guestID
								select b;

					guest = query.SingleOrDefault();
				}
			}

			if( guest != null )
			{
				HydrateCountList();
				// HydrateElements();
			}
		}
	}

	protected void ButtonLookup_Click( object sender, EventArgs e )
	{
		using( WeddingContext db = WeddingContext.New() )
		{
			Debug.Assert( db.DatabaseExists() );

			String searchString = this.TextBoxPartyName.Text.Trim()
										.Replace( " and ", " & ")
										.Replace( " + ", " & ");

			// Find person from db
			var query = from b in db.Guest
						where b.GuestName == searchString
						&& b.ZipCode == this.TextBoxZipCode.Text.Trim()
						select b;

			guest = query.SingleOrDefault();

			if( guest != null )
			{
				HydrateElements();
					//HydrateCountList();
				//this.DivRsvpFindGuest.Visible = false;
				//this.DivRsvpDetails.Visible = true;

				//Session[ "GuestID" ] = guest.GuestID.ToString();

				//if( guest._Count.HasValue )
				//{
				//	// Guest has already RSVP'd, this is an update
				//	this.RadioCount.SelectedIndex = guest._Count.Value;
				//	this.RadioAccommodations.SelectedValue = guest._NeedAccommodations?.ToString();
				//	this.RadioArrival.SelectedValue = guest._ArrivalDay;
				//	this.RadioTravel.SelectedValue = guest._Travel?.ToString();
				//	this.TextBoxNotes.Text = guest._Notes;
				//	this.HyperLinkLodging.Text = guest.Lodging;
				//	this.HyperLinkLodging.NavigateUrl = guest.LodgingUri;
				//	this.LabelCost.Text = guest.Cost.ToString();
				//	this.LabelPaymentReceived.Text = ( guest.IsPaid.HasValue && guest.IsPaid.Value )
				//									? "<span style=\"color:green;font-weight:bold;font-size:14pt;\">Yes!</span></br>Thank you! :)"
				//									: "<span style=\"color:red;font-weight:bold;font-size:14pt;\">No</span></br>(Please PayPal to dave@tamasi.com<br/>so we can pay the resort)";
				//}
			}
			else
			{
				this.DivWarning.Visible = true;
				this.LabelWarning.Text = @"Hmmm, we can't seem to figure out who you are.  Please make sure
										you're typing in your name and ZIP exactly as it appears on your envelope.
										Please try again, or else shoot Dave or Ellen an email, sorry for the pain!";
			}
		}
	}

	protected void ButtonSave_Click( object sender, EventArgs e )
	{
		using( WeddingContext db = WeddingContext.New() )
		{
			Debug.Assert( db.DatabaseExists() );
			Debug.Assert( guestID != Guid.Empty );

			var query = from b in db.Guest
						where b.GuestID == guestID
						select b;

			Guest guest1 = query.Single();

			//if( !this.IsLodging )
			//{
			//	guest1._NeedAccommodations = Byte.Parse( this.RadioAccommodations.SelectedValue );
			//}
			guest1._Count = Byte.Parse( this.RadioCount.SelectedValue );
			guest._Count = Byte.Parse( this.RadioCount.SelectedValue );
			guest1._ArrivalDay = this.RadioArrival.SelectedValue;

			if( !String.IsNullOrEmpty( this.RadioTravel.SelectedValue ) )
			{
				guest1._Travel = Byte.Parse( this.RadioTravel.SelectedValue );
			}
			guest1._Notes = this.TextBoxNotes.Text;
			guest1._RsvpDate = DateTime.Now;
			db.SubmitChanges();
		}

		if( this.IsAttending )
		{
			this.DivRsvpArrivalTravel.Visible = true;
			this.DivAccommodations.Visible = true;
		}
		else
		{
			this.DivRsvpArrivalTravel.Visible = false;
			this.DivAccommodations.Visible = false;
		}


		this.LabelSavedMessage.Text = "Got it -- thanks for letting us know!";
	}

	private void HydrateElements()
	{

		HydrateCountList();
		this.DivRsvpFindGuest.Visible = false;
		this.DivRsvpFoundGuest.Visible = true;
		this.DivRsvpAttendance.Visible = true;
		this.DivRsvpShoutSave.Visible = true;

		Session[ "GuestID" ] = guest.GuestID.ToString();

		if( guest._Count.HasValue )
		{
			// Guest has already RSVP'd, this is an update
			this.RadioCount.SelectedIndex = guest._Count.Value;

			if( this.IsAttending )
			{
				this.DivRsvpArrivalTravel.Visible = true;
				this.DivAccommodations.Visible = true;

				//this.RadioAccommodations.SelectedValue = guest._NeedAccommodations?.ToString();
				this.RadioArrival.SelectedValue = guest._ArrivalDay;
				this.RadioTravel.SelectedValue = guest._Travel?.ToString();
				this.TextBoxNotes.Text = guest._Notes;
				this.HyperLinkLodging.Text = guest.Lodging;
				this.HyperLinkLodging.NavigateUrl = guest.LodgingUri;
				this.LabelCost.Text = guest.Cost.ToString();
				this.LabelPaymentReceived.Text = ( guest.IsPaid.HasValue && guest.IsPaid.Value )
												? "<span style=\"color:green;font-weight:bold;font-size:14pt;\">Yes!</span></br>Thank you! :)"
												: "<span style=\"color:red;font-weight:bold;font-size:14pt;\">No</span></br>(Please PayPal to dave@tamasi.com<br/>so we can pay the resort)";
			}
			else
			{
				this.DivRsvpArrivalTravel.Visible = false;
				this.DivAccommodations.Visible = false;

			}
		}
	}



	private void HydrateCountList()
	{
		if( this.RadioCount.Items.Count == 0 )
		{
			for( Byte k = 0; k <= guest.MaxSize; k++ )
			{
				String text = k == 0
					? "&nbsp;&nbsp;0 (not attending)"
					: String.Format( "&nbsp;&nbsp;{0}", k );
				ListItem li = new ListItem( text, k.ToString() );
				this.RadioCount.Items.Add( li );
			}
		}
	}
}