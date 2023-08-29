using Blackbird.Applications.Sdk.Common.Authentication;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;

namespace Apps.Box
{
    public class BlackbirdBoxClient : BoxClient
    {
        public BlackbirdBoxClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) 
            : base(GetConfig(), GetSession(authenticationCredentialsProviders)) { }
        
        private static IBoxConfig GetConfig() 
            => new BoxConfigBuilder(ApplicationConstants.ClientId, ApplicationConstants.ClientSecret, 
                new Uri(ApplicationConstants.RedirectUri)).Build();

        private static OAuthSession GetSession(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var accessToken = authenticationCredentialsProviders.First(p => p.KeyName == "access_token").Value;
            return new OAuthSession(accessToken, "N/A", 3600, "bearer");
        }
    }
}
