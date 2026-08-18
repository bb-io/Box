using System.Net;
using Apps.Box.Constants;
using Apps.Box.Events.Webhooks.Handlers.Files;
using Apps.Box.Extensions;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Box.Events.Webhooks;

[WebhookList("Files")]
public class FileWebhookList(InvocationContext invocationContext) : BoxInvocable(invocationContext)
{
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdated)]
    [Webhook("On file created", typeof(FileUploadedHandler),
        Description = "Triggered when a file is uploaded to the selected folder or one of its subfolders")]
    public Task<WebhookResponse<FileEventResponse>> OnFileCreated(WebhookRequest webhookRequest,
        [WebhookParameter] ParentFolderFilterRequest parentFolder)
        => Task.FromResult(HandleFileTrigger(webhookRequest, BoxWebhookTriggers.FileUploaded, parentFolder));

    private static WebhookResponse<FileEventResponse> HandleFileTrigger(WebhookRequest webhookRequest, string trigger,
        ParentFolderFilterRequest? parentFolder)
    {
        var payload = webhookRequest.GetPayload();

        if (payload.Trigger != trigger || payload.Source is null)
            return Preflight();

        var parentFolderId = parentFolder?.ParentFolderId;
        if (!string.IsNullOrWhiteSpace(parentFolderId) && payload.Source.Parent?.Id != parentFolderId.Trim())
            return Preflight();

        return Fly(new FileEventResponse(payload.Source));
    }

    private static WebhookResponse<FileEventResponse> Fly(FileEventResponse result)
        => new()
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = result
        };

    private static WebhookResponse<FileEventResponse> Preflight()
        => new()
        {
            HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
            Result = null,
            ReceivedWebhookRequestType = WebhookRequestType.Preflight
        };
}
