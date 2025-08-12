using Blackbird.Applications.Sdk.Common;
using Box.V2.Models;

namespace Apps.Box.Dtos;

public abstract class BoxItemDto
{
    protected BoxItemDto(BoxItem item)
    {
        Path = string.Join('/', item.PathCollection.Entries.Select(p => p.Name)) + "/";
        Name = item.Name;
        Size = item.Size;
        Description = item.Description;
        ParentFolderId = item.Parent?.Id ?? "" ;
        CreatedBy = item.CreatedBy?.Name ?? "";
    }
    
    public string Path { get; set; }
    public string Name { get; set; }
    public long? Size { get; set; }
    public string Description { get; set; }

    [Display("Parent folder ID")]
    public string ParentFolderId { get; set; }

    [Display("Created by")]
    public string CreatedBy { get; set; }
}