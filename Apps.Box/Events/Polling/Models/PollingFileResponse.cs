using Blackbird.Applications.Sdk.Common;
using Box.V2.Models;

namespace Apps.Box.Events.Polling.Models;

public class PollingFileResponse
{
    [Display("File ID")]
    public string FileId { get; set; }
    public string Path { get; set; }
    public string Name { get; set; }
    public long? Size { get; set; }
    public string Description { get; set; }
    
    public PollingFileResponse(BoxItem item)
    {
        FileId = item.Id;
        Path = string.Join('/', item.PathCollection.Entries.Select(p => p.Name)) + "/";
        Name = item.Name;
        Size = item.Size;
        Description = item.Description;
    }
}