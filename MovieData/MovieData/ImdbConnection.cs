using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Tamasi.MovieData
{
	public static class ImdbConnection
	{
		#region Methods

		public static ImdbMovie GetImdbMovie( String movieName )
		{
			ImdbMovie movie = null;
			String requestName = movieName.Replace(" ", "+" );
			HttpResponseMessage response = null;

			using( HttpClient client = new HttpClient() )
			{
				client.BaseAddress = new Uri( "http://www.omdbapi.com" );
				client.DefaultRequestHeaders.Accept.Clear();
				client.DefaultRequestHeaders.Accept.Add( new MediaTypeWithQualityHeaderValue( "application/json" ) );
				response = client.GetAsync( String.Format( "http://www.omdbapi.com/?apikey=526156d3&t={0}", requestName ) ).Result;
			}

			if( response.IsSuccessStatusCode )
			{
				movie = response.Content.ReadAsAsync<ImdbMovie>().Result;
			}

			return movie;
		}

		#endregion
	}
}
