using Newtonsoft.Json;

namespace Apps.Box.Models.Payloads;

public class BoxWebhookPayload
{
    [JsonProperty("trigger")]
    public string Trigger { get; set; } = string.Empty;

    [JsonProperty("source")]
    public BoxWebhookSource? Source { get; set; }
}
