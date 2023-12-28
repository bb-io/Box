
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Box.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
    {
            //var client = new BlackbirdBoxClient(authProviders, "");
            //try
            //{
            //    await client.CollectionsManager.GetCollectionsAsync();

            //}
            //catch (Exception ex)
            //{
            //    return new ConnectionValidationResponse
            //    {
            //        IsValid = false,
            //        Message = ex.Message
            //    };
            //}
            return new ConnectionValidationResponse
            {
                IsValid = true,
                Message = "Success"
            };
        }
}