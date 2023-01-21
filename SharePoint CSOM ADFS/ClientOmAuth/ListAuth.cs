using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientOmAuth.listsWS
{
	public partial class Lists : System.Web.Services.Protocols.SoapHttpClientProtocol
	{
		protected override System.Net.WebRequest GetWebRequest(Uri uri)
		{
			System.Net.WebRequest wr = null;

			try
			{
				wr = base.GetWebRequest(uri);
				wr.Headers.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
			}
			catch (Exception ex)
			{
				//some error handling here
			}
				
			return wr;
		}
	}
}
