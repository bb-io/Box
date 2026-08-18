using Apps.Box.Models.Payloads;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Box.Extensions;

public static class WebhookRequestExtensions
{
    public static BoxWebhookPayload GetPayload(this WebhookRequest webhookRequest)
    {
        var payload = JsonConvert.DeserializeObject<BoxWebhookPayload>(webhookRequest.Body.ToString()!);
        if (payload is null)
            throw new InvalidCastException(nameof(webhookRequest.Body));

        return payload;
    }
}
