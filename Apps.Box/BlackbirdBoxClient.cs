using Blackbird.Applications.Sdk.Common.Authentication;
using Box.V2;
using Box.V2.Auth;
using Box.V2.Config;

namespace Apps.Box;

public class BlackbirdBoxClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, string redirectUri)
    : BoxClient(GetConfig(redirectUri), GetSession(authenticationCredentialsProviders))
{
    private static IBoxConfig GetConfig(string redirectUri) 
        => new BoxConfigBuilder(ApplicationConstants.ClientId, ApplicationConstants.ClientSecret, 
            new Uri(redirectUri)).Build();

    private static OAuthSession GetSession(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
    {
        var accessToken = authenticationCredentialsProviders.First(p => p.KeyName == "access_token").Value;
        return new OAuthSession(accessToken, "N/A", 3600, "bearer");
    }
}