using Newtonsoft.Json;

namespace Apps.Box.Models.Entities;

public class BoxWebhookEntity
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
