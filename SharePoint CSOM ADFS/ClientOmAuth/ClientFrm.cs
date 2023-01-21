using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.IO;
using Microsoft.SharePoint.Client;
using System.Web;

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Security.Principal;
using System.IdentityModel.Protocols.WSTrust;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace ClientOmAuth
{
    public partial class ClientFrm : System.Windows.Forms.Form
    {

        List<string> outputList;

        public ClientFrm()
        {
            InitializeComponent();
            outputList = new List<string>();
        }


        //web request used for setting up authorization with claims auth sites
        HttpWebRequest wr = null;

        //web response for working with data returned from web request
        HttpWebResponse resp = null;

        




        private void SamlListBtn_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;


                ServicePointManager.ServerCertificateValidationCallback = delegate (object sender1, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    bool validationResult = true;
                    return validationResult;
                };




                ClientContext ctx = new ClientContext(SamlTxt.Text);


                //use default credentials
                //ctx.Credentials = CredentialCache.DefaultCredentials;

                ctx.Credentials = new NetworkCredential(txtUserName.Text, txtPassword.Text, txtDomain.Text);

                ctx.RequestTimeout = 5000; // 1 minute.

        

                //configure the handler that will pick up the auth cookie
                ctx.ExecutingWebRequest += new EventHandler<WebRequestEventArgs>(ctx_ExecutingWebRequest);

                //get the web
                Web w = ctx.Web;

                ctx.Load(w);

                //LOAD LISTS WITH ALL PROPERTIES
                var lists = ctx.LoadQuery(w.Lists);

                //execute the query
                ctx.ExecuteQuery();

                //MessageBox.Show(w.Title);

                FbaLst.Items.Clear();
                foreach (List theList in lists)
                {
                    txtResults.Text += theList.ToString();
                }

                this.Cursor = Cursors.Default;

    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving data: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        void ctx_ExecutingWebRequest(object sender, WebRequestEventArgs e)
        {

            try
            {
                string samlToken = GetSamlToken();

                txtCookie.Text = samlToken;

                if (!string.IsNullOrEmpty(samlToken))
                {
                    CookieContainer cc = new CookieContainer();
                    Cookie samlAuth = new Cookie("FedAuth", samlToken);
                    samlAuth.Expires = DateTime.Now.AddHours(1);
                    samlAuth.Path = "/";
                    samlAuth.Secure = true;
                    samlAuth.HttpOnly = true;
                    Uri samlUri = new Uri(SamlTxt.Text);
                    samlAuth.Domain = samlUri.Host;
                    cc.Add(samlAuth);
                    e.WebRequestExecutor.WebRequest.CookieContainer = cc;
                }

                
                outputList.Add(e.WebRequestExecutor.WebRequest.Address.ToString());
                CookieCollection collection = e.WebRequestExecutor.WebRequest.CookieContainer.GetCookies(e.WebRequestExecutor.WebRequest.Address);

                foreach (Cookie c in collection)
                {
                    outputList.Add(c.ToString());
                }

  
                txtOutput.Lines = outputList.ToArray();


                //txtOutput.Text += e.WebRequestExecutor.WebRequest.CookieContainer.GetCookies(e.WebRequestExecutor.WebRequest.Address)["FedAuth"].ToString();


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error setting auth cookie: " + ex.Message);
            }
        }


        private string GetFormValue(string key, string source)
        {
            string ret = string.Empty;

            try
            {
                //first find the name for the input
                int nameTag = source.IndexOf("name=\"" + key);

                //now get the value attribute after that
                int valueTag = source.IndexOf("value=\"", nameTag);

                //now look for the closing quote mark
                int quoteTag = source.IndexOf("\"", valueTag + 7);

                //now get the part in the middle
                ret = source.Substring(valueTag + 7, quoteTag - valueTag - 7);

                //now decode it so ampersands won't throw off the processing of form variables
                //turn it into valid html before decoding or you end up double-encoding and
                //everything will fail then - causes a 500 internal server error
                ret = System.Web.HttpUtility.HtmlDecode(ret);
                ret = System.Web.HttpUtility.UrlEncode(ret);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return ret;
        }


        private HttpWebRequest MakeRequest(string Url, string RequestMethod, string responseCookies)
        {
            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(Url);
            wr.UseDefaultCredentials = true;
            wr.Method = RequestMethod;
            wr.Accept = "*/*";
            wr.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E; .NET CLR 3.5.30729; .NET CLR 3.0.30729; InfoPath.3)";
            wr.Headers.Add("Accept-Language", "en-US");
            wr.Headers.Add("Cache-Control", "no-cache");
            wr.KeepAlive = true;
            wr.PreAuthenticate = true;
            wr.AllowAutoRedirect = true;

            //if it's a post then set the correct content type
            if (RequestMethod.ToLower() == "post")
                wr.ContentType = "application/x-www-form-urlencoded";
            
            //process cookies; they are comma delimited
            if (!string.IsNullOrEmpty(responseCookies))
                wr.Headers.Add("Set-Cookie", responseCookies);

            return wr;
        }


        private void SendPagePost(string Url, bool AutoRedirect, string[] FormFields, 
            Dictionary<string, string> FixedFieldValues)
        {
            try
            {
                //create the encoding to be used when reading the page
                Encoding enc = Encoding.GetEncoding("utf-8");

                //open a stream to read the page and stuff it in a string
                StreamReader thePage = new StreamReader(resp.GetResponseStream(), enc);
                string pageContents = thePage.ReadToEnd();
                thePage.Close();
                resp.Close();

                //create our new web request and use header cookies if found
                string headerCookies = resp.GetResponseHeader("Set-Cookie");
                wr = MakeRequest(Url, "POST", headerCookies);

                //after we post back to the /_trust/ subdirectory we no 
                //longer want to redirect because we'll get our FedAuth
                //cookie once we get back
                wr.AllowAutoRedirect = AutoRedirect;

                string postData = string.Empty;

                //get the form values out and post back
                for(int i = 0; i < FormFields.Length; i++)
                {
                    if (i > 0)
                        postData += "&";
                    postData += FormFields[i] + "=" + GetFormValue(FormFields[i], pageContents);
                }

                //add any fixed field values
                if (FixedFieldValues != null)
                {
                    foreach (string key in FixedFieldValues.Keys)
                    {
                        postData += "&" + key + "=" + FixedFieldValues[key];
                    }                
                }
                
                //set the length of post data
                wr.ContentLength = postData.Length;

                //send the data
                Stream postStream = wr.GetRequestStream();
                byte[] postByte = Encoding.UTF8.GetBytes(postData);
                postStream.Write(postByte, 0, postByte.Length);
                postStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error doing authorization post back: " + ex.Message);
            }
        }


        private string GetSamlToken()
        {
            string ret = string.Empty;

            try
            {
                string samlServer = SamlTxt.Text.EndsWith("/") ? SamlTxt.Text : SamlTxt.Text + "/";

                var sharepointSite = new
                {
                    Wctx = samlServer + "_layouts/Authenticate.aspx?Source=%2F",
                    Wtrealm = samlServer,
                    Wreply = samlServer + "_trust/"
                };

                string stsServer = AdfsTxt.Text.EndsWith("/") ? AdfsTxt.Text : AdfsTxt.Text + "/";
				//string stsUrl = stsServer + "adfs/services/trust/2005/windowstransport";
				string stsUrl = stsServer + "adfs/services/trust/13/windowstransport";

                //get token from STS
                string stsResponse = GetResponse(stsUrl, sharepointSite.Wreply);
                //txtOutput.Text += stsResponse;
                //generate response to Sharepoint
                string stringData = String.Format("wa=wsignin1.0&wctx={0}&wresult={1}",
                    HttpUtility.UrlEncode(sharepointSite.Wctx),
                    HttpUtility.UrlEncode(stsResponse));
                HttpWebRequest sharepointRequest = HttpWebRequest.Create(sharepointSite.Wreply) as HttpWebRequest;
                sharepointRequest.Method = "POST";
                sharepointRequest.ContentType = "application/x-www-form-urlencoded";
                sharepointRequest.CookieContainer = new CookieContainer();
                sharepointRequest.AllowAutoRedirect = false; // This is important
                Stream newStream = sharepointRequest.GetRequestStream();

                byte[] data = Encoding.UTF8.GetBytes(stringData);
                newStream.Write(data, 0, data.Length);
                newStream.Close();
                HttpWebResponse webResponse = sharepointRequest.GetResponse() as HttpWebResponse;
                ret = webResponse.Cookies["FedAuth"].Value;
                //txtOutput.Text += ret;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return ret;
        }


        private string GetResponse(string stsUrl, string realm)
        {

            RequestSecurityToken rst = new RequestSecurityToken();
            //rst.RequestType = WSTrustFeb2005Constants.RequestTypes.Issue;
			rst.RequestType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue";


            //bearer token, no encryption
            rst.AppliesTo = new EndpointReference(realm);
            //rst.KeyType = WSTrustFeb2005Constants.KeyTypes.Bearer;
			rst.KeyType = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer";


            //WSTrustFeb2005RequestSerializer trustSerializer = new WSTrustFeb2005RequestSerializer();
            WSTrust13RequestSerializer trustSerializer = new WSTrust13RequestSerializer();
			WSHttpBinding binding = new WSHttpBinding();

            binding.Security.Mode = SecurityMode.Transport;

            binding.Security.Message.ClientCredentialType = MessageCredentialType.None;
            binding.Security.Message.EstablishSecurityContext = false;

            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
            EndpointAddress address = new EndpointAddress(stsUrl);
			//WSTrustFeb2005ContractClient trustClient = new WSTrustFeb2005ContractClient(binding, address);
			WSTrust13ContractClient trustClient = new WSTrust13ContractClient(binding, address);

            trustClient.ClientCredentials.Windows.AllowNtlm = true;
            trustClient.ClientCredentials.Windows.AllowedImpersonationLevel = TokenImpersonationLevel.Impersonation;

            // Does this need updating to include custom creds?
            //
            //
            //
            //trustClient.ClientCredentials.Windows.ClientCredential = CredentialCache.DefaultNetworkCredentials;
            trustClient.ClientCredentials.Windows.ClientCredential = new System.Net.NetworkCredential(txtUserName.Text, txtPassword.Text, txtDomain.Text);
            //
            //
            //
            //


            System.ServiceModel.Channels.Message response =
                trustClient.EndIssue(trustClient.BeginIssue(
                System.ServiceModel.Channels.Message.CreateMessage(
                //MessageVersion.Default, WSTrustFeb2005Constants.Actions.Issue, 
				MessageVersion.Default, "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue",
                new RequestBodyWriter(trustSerializer, rst)), null, null));
            trustClient.Close();
      
            XmlDictionaryReader reader = response.GetReaderAtBodyContents();
            return reader.ReadOuterXml();
        }



        private void GetRestDataBtn_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                //ex:  https://fc1/_vti_bin/listdata.svc/Contacts should show 
                //all items in the Contacts list

                //this is the REST endpoint we want to use to get all Contacts
                string endpoint = "https://fc1/_vti_bin/listdata.svc/Contacts";

                //get the FedAuth cookie
                string FedAuth = GetSamlToken();

                //make a request to the REST interface for the data
                HttpWebRequest webRqst = (HttpWebRequest)WebRequest.Create(endpoint);
                webRqst.UseDefaultCredentials = true;
                webRqst.Method = "GET";
                webRqst.Accept = "*/*";
                webRqst.KeepAlive = true;

                //create the FedAuth cookie that will go with our request
                CookieContainer cc = new CookieContainer();
                Cookie samlAuth = new Cookie("FedAuth", FedAuth);
                samlAuth.Expires = DateTime.Now.AddHours(1);
                samlAuth.Path = "/";
                samlAuth.Secure = true;
                samlAuth.HttpOnly = true;
                Uri samlUri = new Uri(SamlTxt.Text);
                samlAuth.Domain = samlUri.Host;
                cc.Add(samlAuth);

                //plug our cookie into the request
                webRqst.CookieContainer = cc;

                //read the response now
                HttpWebResponse webResp = webRqst.GetResponse() as HttpWebResponse;

                //make the request and get the response
                StreamReader theData = new StreamReader(webResp.GetResponseStream(), true);
                string payload = theData.ReadToEnd();
                theData.Close();
                webResp.Close();

                //create the Xml classes for working with the results

                //xml doc, loaded with the results
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(payload);

                //namespace manager, used for querying
                XmlNamespaceManager ns = new XmlNamespaceManager(xDoc.NameTable);
                ns.AddNamespace("b",
                    "http://www.w3.org/2005/Atom");
                ns.AddNamespace("m",
                    "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
                ns.AddNamespace("d",
                    "http://schemas.microsoft.com/ado/2007/08/dataservices");

                //query for items
                XmlNodeList nl = xDoc.SelectNodes("/b:feed/b:entry/b:content/m:properties", ns);

                //create a list to hold the results
                List<Contact> Contacts = new List<Contact>();

                //enumerate the results
                foreach (XmlNode xNode in nl)
                {
                    Contacts.Add(new Contact(
                        xNode.SelectSingleNode("d:FirstName", ns).InnerText,
                        xNode.SelectSingleNode("d:LastName", ns).InnerText,
                        xNode.SelectSingleNode("d:Company", ns).InnerText,
                        xNode.SelectSingleNode("d:JobTitle", ns).InnerText,
                        xNode.SelectSingleNode("d:EMailAddress", ns).InnerText));
                }

                //bind to the grid
                //ContactGrd.DataSource = Contacts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        public class Contact
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Company { get; set; }
            public string JobTitle { get; set; }
            public string Email { get; set; }

            public Contact(string First, string Last, string Company, string Title, string Email)
            {
                this.FirstName = First;
                this.LastName = Last;
                this.Company = Company;
                this.JobTitle = Title;
                this.Email = Email;
            }
        }




		void ctx_MixedAuthRequest(object sender, WebRequestEventArgs e)
		{
			try
			{
				//add the header that tells SharePoint to use Windows Auth
				e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error setting auth header: " + ex.Message);
			}
		}

        #region Non-Optimized Code

        //follow up #1 - at the ADFS site
        //we should be at the ADFS login page; read in so 

        //thePage = new StreamReader(resp.GetResponseStream(), true);
        //pageContents = thePage.ReadToEnd();
        //thePage.Close();
        //resp.Close();

        ////we have the page contents - create a post back
        //wr = MakeRequest(resp.ResponseUri.AbsoluteUri, "POST", string.Empty);

        ////need to get the following form values out and post back
        ////__VIEWSTATE
        ////__EVENTVALIDATION
        ////__db
        ////ctl00$ContentPlaceHolder1$__ic=autoLogonFailed
        //string postData = "__VIEWSTATE=" + GetFormValue("__VIEWSTATE", pageContents);
        //postData += "&__EVENTVALIDATION=" + GetFormValue("__EVENTVALIDATION", pageContents);
        //postData += "&__db=" + GetFormValue("__db", pageContents);
        //postData += "&ctl00$ContentPlaceHolder1$__ic=autoLogonFailed";

        //wr.ContentLength = postData.Length;

        ////send the data
        //Stream postStream = wr.GetRequestStream();
        //byte[] postByte = Encoding.UTF8.GetBytes(postData);
        //postStream.Write(postByte, 0, postByte.Length);
        //postStream.Close();

        //second follow up - ready to post to the /_trust/ subdirectory

        //we've posted back to the ADFS site and should have the cookies
        //need to authenticated to the SharePoint site now
        //we should be at the ADFS login page; read in so 
        //we know how to get redirected

        //Encoding enc = Encoding.GetEncoding("utf-8");
        //thePage = new StreamReader(resp.GetResponseStream(), enc);
        //pageContents = thePage.ReadToEnd();
        //thePage.Close();
        //resp.Close();

        //headerCookies = resp.GetResponseHeader("Set-Cookie");
        //wr = MakeRequest(redir + "/_trust/", "POST", headerCookies);

        ////after we post back to the /_trust/ subdirectory we no 
        ////longer want to redirect because we'll get our FedAuth
        ////cookie once we get back.  So we'll turn off auto redirect
        ////so we can capture our cookie after the next request
        //wr.AllowAutoRedirect = false;

        ////need to get the following form values out and post back
        ////wa
        ////wresult
        ////wctx
        //string postData = "wa=" + GetFormValue("wa", pageContents);
        //postData += "&wresult=" + GetFormValue("wresult", pageContents);
        //postData += "&wctx=" + GetFormValue("wctx", pageContents);

        //wr.ContentLength = postData.Length;

        ////send the data
        //Stream postStream = wr.GetRequestStream();
        //byte[] postByte = Encoding.UTF8.GetBytes(postData);
        //postStream.Write(postByte, 0, postByte.Length);
        //postStream.Close();

        //freakishly large error handler
        //Debug.WriteLine(getEx.Message);
        //HttpWebResponse response = (HttpWebResponse)authEx.Response;
        //if (response != null)
        //{
        //    if ((response.StatusCode == HttpStatusCode.Moved) ||
        //        (response.StatusCode == HttpStatusCode.MovedPermanently) ||
        //        (response.StatusCode == HttpStatusCode.Redirect))
        //        redir = response.GetResponseHeader("Location");
        //    else if (response.StatusCode == HttpStatusCode.Unauthorized)
        //        redir = response.ResponseUri.AbsoluteUri;
        //    else if (response.StatusCode == HttpStatusCode.Forbidden)
        //        redir = response.GetResponseHeader("X-Forms_Based_Auth_Required");
        //    else if (response.StatusCode == HttpStatusCode.InternalServerError)
        //        foundSite = true;

        //    //get any cookies included in the header
        //    headerCookies = resp.GetResponseHeader("Set-Cookie");
        //}
        //else
        //foundSite = true;  //never gonna exit loop otherwise

        //wr = MakeRequest(redir, "GET", headerCookies);

        #endregion

        private void SamlGroup_Enter(object sender, EventArgs e)
        {

        }

        private void SamlTxt_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
