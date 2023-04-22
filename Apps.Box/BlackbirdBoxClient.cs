using Blackbird.Applications.Sdk.Common.Authentication;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box
{
    public class BlackbirdBoxClient : BoxClient
    {
        private static IBoxConfig GetConfig(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var clientId = authenticationCredentialsProviders.First(p => p.KeyName == "clientId").Value;
            var clientSecret = authenticationCredentialsProviders.First(p => p.KeyName == "clientSecret").Value;
            return new BoxConfigBuilder(clientId, clientSecret, new Uri("http://localhost")).Build();
        }

        private static OAuthSession GetSession(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var devToken = authenticationCredentialsProviders.First(p => p.KeyName == "devToken").Value;
            return new OAuthSession(devToken, "N/A", 3600, "bearer");
        }
        public BlackbirdBoxClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base(GetConfig(authenticationCredentialsProviders), GetSession(authenticationCredentialsProviders)) { }
    }
}
