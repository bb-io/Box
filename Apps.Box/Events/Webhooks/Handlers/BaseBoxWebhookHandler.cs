using Apps.Box.Models.Entities;
using Apps.Box.Models.Requests;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Box.V2.Models;

namespace Apps.Box.Events.Webhooks.Handlers;

public abstract class BaseBoxWebhookHandler(
    InvocationContext invocationContext,
    [WebhookParameter(isSubscriptionDepends: true)] WebhookFolderRequest folder)
    : BoxInvocable(invocationContext), IWebhookEventHandler
{
    private const string PayloadUrlKey = "payloadUrl";
    private const string RootFolderId = "0";
    private const int WebhooksPageSize = 100;

    protected abstract IReadOnlyCollection<string> Triggers { get; }

    public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
        => ExecuteWithErrorHandlingAsync(() =>
            CreateWebhookAsync(CreateClient(authenticationCredentialsProvider), CreateSubscription(values)));

    public Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProvider,
        Dictionary<string, string> values)
        => ExecuteWithErrorHandlingAsync(() =>
            DeleteWebhookAsync(CreateClient(authenticationCredentialsProvider), CreateSubscription(values)));

    private static async Task CreateWebhookAsync(BlackbirdBoxClient client, BoxWebhookSubscription subscription)
    {
        var request = new BoxWebhookRequest
        {
            Target = new BoxRequestEntity
            {
                Id = subscription.FolderId,
                Type = BoxType.folder
            },
            Address = subscription.CallbackUrl,
            Triggers = subscription.Triggers.ToList()
        };

        await client.WebhooksManager.CreateWebhookAsync(request);
    }

    private static async Task DeleteWebhookAsync(BlackbirdBoxClient client, BoxWebhookSubscription subscription)
    {
        var webhook = await FindWebhookAsync(client, subscription);
        if (webhook is null)
            return;

        await client.WebhooksManager.DeleteWebhookAsync(webhook.Id);
    }

    private static async Task<BoxWebhook?> FindWebhookAsync(BlackbirdBoxClient client,
        BoxWebhookSubscription subscription)
    {
        string? marker = null;

        do
        {
            var page = await client.WebhooksManager.GetWebhooksAsync(WebhooksPageSize, marker);

            foreach (var entry in page.Entries)
            {
                var webhook = await client.WebhooksManager.GetWebhookAsync(entry.Id);
                if (Matches(webhook, subscription))
                    return webhook;
            }

            marker = page.NextMarker;
        } while (!string.IsNullOrEmpty(marker));

        return null;
    }

    private static bool Matches(BoxWebhook webhook, BoxWebhookSubscription subscription)
        => webhook.Address == subscription.CallbackUrl
           && subscription.Triggers.All(trigger => webhook.Triggers.Contains(trigger));

    private BlackbirdBoxClient CreateClient(IEnumerable<AuthenticationCredentialsProvider> credentials)
        => new(credentials, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());

    private BoxWebhookSubscription CreateSubscription(Dictionary<string, string> values)
        => new(GetFolderId(), GetCallbackUrl(values), Triggers);

    private string GetFolderId()
    {
        if (string.IsNullOrWhiteSpace(folder.FolderId))
            throw new PluginMisconfigurationException("Folder is not specified. Please select a folder and try again");

        var folderId = folder.FolderId.Trim();
        if (folderId == RootFolderId)
            throw new PluginMisconfigurationException(
                "Box does not allow subscriptions on the root folder. Please select a specific folder and try again");

        return folderId;
    }

    private static string GetCallbackUrl(Dictionary<string, string> values)
    {
        if (!values.TryGetValue(PayloadUrlKey, out var callbackUrl) || string.IsNullOrWhiteSpace(callbackUrl))
            throw new PluginApplicationException("Blackbird did not provide a callback URL for this subscription");

        return callbackUrl;
    }
}
