using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace soapgateQCSharpDemo
{
    public class Webservice
    {
        public soapgateQ.DominoUtilityWebServices Client;
        private string rootUrl;
        private string wsDb;
        private CookieContainer cookieContainer;

        public Webservice(string rootUrl, string wsDb)
        {
            this.Client = new soapgateQ.DominoUtilityWebServices(rootUrl);
            this.cookieContainer = new CookieContainer();
            this.Client.CookieContainer = this.cookieContainer;
            this.rootUrl = rootUrl;
            this.wsDb = wsDb;
        }

        public LoginResult Login(string username, string password)
        {
            return this.Login(username, password, this.rootUrl, this.wsDb);
        }

        public LoginResult Login(string username, string password, string rootUrl, string wsDb)
        {
            string postdata = "username=" + HttpUtility.UrlEncode(username) + "&password=" + HttpUtility.UrlEncode(password) + "&redirectto=" + HttpUtility.UrlEncode(wsDb + "/Database Info");
            byte[] data = Encoding.Default.GetBytes(postdata);
            string buffer;
            LoginResult lr = new LoginResult();

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(rootUrl + "/names.nsf?Login");
            request.CookieContainer = this.cookieContainer;
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:20.0) Gecko/20100101 Firefox/20.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language: de-de,de;q=0.8,en-us;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip, deflate");
            request.ContentLength = data.Length;

            using(Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream responseStream = (response.ContentEncoding.ToLower().Contains("gzip") ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream()))

            using(StreamReader srBuffer = new StreamReader(responseStream, Encoding.Default))
            {
                buffer = srBuffer.ReadToEnd();
            }

            if(buffer.Contains("product"))
            {
                XDocument xDoc = XDocument.Parse(buffer);
                lr.Successful = true;
                lr.Version = xDoc.Element("root").Element("soapgateq").Element("version").Value;
                lr.WebServiceEndPoint = rootUrl + wsDb + "/dominoutilitywebservices?OpenWebService";
                this.Client.Url = lr.WebServiceEndPoint;

                Debug.WriteLine(string.Format("Login successful - WebService-URL: {0}", lr.WebServiceEndPoint));
                Debug.WriteLine(string.Format("soapgateQ! Version: {0}", lr.Version));
            }
            else
            {
                lr.Successful = false;
                Debug.WriteLine("Login failed");
            }

            return lr;
        }

        public bool Logout()
        {
            return this.Logout(this.rootUrl, this.wsDb);
        }

        public bool Logout(string rootUrl, string wsDb)
        {
            string url = rootUrl + "/names.nsf?Logout&redirectto=" + HttpUtility.UrlEncode(wsDb) + "/Database%20Info";
            bool result = false;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = this.cookieContainer;

            request.Timeout = 60000;
            request.Method = "GET";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:20.0) Gecko/20100101 Firefox/20.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.ContentType = "application/x-www-form-urlencoded";
            request.Headers.Add("Accept-Language: de-de,de;q=0.8,en-us;q=0.5,en;q=0.3");
            request.Headers.Add("Accept-Encoding: gzip, deflate");

            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream responseStream = (response.ContentEncoding.ToLower().Contains("gzip") ? new GZipStream(response.GetResponseStream(), CompressionMode.Decompress) : response.GetResponseStream()))

            using(StreamReader buffer = new StreamReader(responseStream, Encoding.Default))
            {
                result = buffer.ReadToEnd().Contains("Server Login");
            }

            if(result)
                Debug.WriteLine("Logout successful");
            else
                Debug.WriteLine("Logout failed");

            return result;
        }

        public string[] MailInfo(string username)
        {
            return this.Client.DBGETMAILINFO(username);
        }
    }
}
