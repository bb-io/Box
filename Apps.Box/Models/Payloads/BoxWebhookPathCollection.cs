using Apps.Box.Models.Entities;
using Newtonsoft.Json;

namespace Apps.Box.Models.Payloads;

public class BoxWebhookPathCollection
{
    [JsonProperty("entries")]
    public IEnumerable<BoxWebhookEntity> Entries { get; set; } = [];
}
