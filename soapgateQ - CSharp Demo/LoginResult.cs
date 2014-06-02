
namespace soapgateQCSharpDemo
{
    public class LoginResult
    {
        public bool Successful { get; set; }
        public string Version { get; set; }
        public string WebServiceEndPoint { get; set; }

        public LoginResult()
        {
            this.Successful = false;
            this.Version = "-";
            this.WebServiceEndPoint = "-";
        }
    }
}
