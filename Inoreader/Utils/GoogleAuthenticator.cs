using System;
using System.Linq;
using RestSharp;

namespace Inoreader.Utils
{
    public class GoogleAuthenticator : IAuthenticator
    {
        const string HeaderName = "Authorization";
        private readonly string _token;

        public GoogleAuthenticator(string token)
        {
            _token = token;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            if (request.Parameters.All(p => !p.Name.Equals(HeaderName, StringComparison.OrdinalIgnoreCase)))
            {
                var value = string.Format("GoogleLogin auth={0}", _token);
                request.AddHeader(HeaderName, value);
            }
        }
    }
}