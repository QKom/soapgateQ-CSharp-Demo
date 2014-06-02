using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace soapgateQCSharpDemo
{
    class Program
    {
        public static string RootUrl = "http://domino.flexdomino.net";

        // To communicate over SSL uncomment the following line and line 21
        //public static string RootUrl = "https://domino.flexdomino.net";

        public static string WsDb = "/soapgateq_5.nsf";
        public static bool AcceptAllCertifications(object sender, X509Certificate certification, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; }

        static void Main(string[] args)
        {
            // Uncomment the following line to ignore invalid SSL certificate 
            //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);

            Webservice ws = new Webservice(RootUrl, WsDb);

            // Session based login against the domino server
            LoginResult lr = ws.Login("Online Demo", "demo");

            // triggers soapgateQ dbGetMailInfo method
            string[] result = ws.MailInfo("odemo");
            foreach(string item in result)
            {
                Console.WriteLine("Value: " + item);
            }

            // destroy the session
            ws.Logout();

            // unauthorized request throws an exception
            result = ws.MailInfo("odemo");
        }
    }
}
