using Apps.Box.Models.Entities;
using Newtonsoft.Json;

namespace Apps.Box.Models.Payloads;

public class BoxWebhookSource
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("size")]
    public long? Size { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonProperty("modified_at")]
    public DateTimeOffset? ModifiedAt { get; set; }

    [JsonProperty("parent")]
    public BoxWebhookEntity? Parent { get; set; }

    [JsonProperty("created_by")]
    public BoxWebhookEntity? CreatedBy { get; set; }

    [JsonProperty("modified_by")]
    public BoxWebhookEntity? ModifiedBy { get; set; }

    [JsonProperty("path_collection")]
    public BoxWebhookPathCollection? PathCollection { get; set; }
}
