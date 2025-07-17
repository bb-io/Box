using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Box.V2.Exceptions;

namespace Apps.Box;

public class BoxInvocable : BaseInvocable
{
    protected IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    protected BlackbirdBoxClient Client { get; }


    public BoxInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());

    }

    protected async Task ExecuteWithErrorHandlingAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (BoxAPIException ex)
        {
            throw new PluginApplicationException(ex.ErrorDescription);
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
    }

    protected async Task<T> ExecuteWithErrorHandlingAsync<T>(Func<Task<T>> action)
    {
        try
        {
            return await action();
        }
        catch (BoxAPIException ex)
        {
            throw new PluginApplicationException(ex.ErrorDescription);
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException(ex.Message);
        }
    }

}