using Apps.Box.Constants;
using Apps.Box.Models.Requests;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Box.Events.Webhooks.Handlers.Files;

public class FileUploadedHandler(
    InvocationContext invocationContext,
    [WebhookParameter(isSubscriptionDepends: true)] WebhookFolderRequest folder)
    : BaseBoxWebhookHandler(invocationContext, folder)
{
    protected override IReadOnlyCollection<string> Triggers => [BoxWebhookTriggers.FileUploaded];
}
