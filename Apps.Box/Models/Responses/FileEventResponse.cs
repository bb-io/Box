using Apps.Box.Models.Payloads;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Box.Models.Responses;

public class FileEventResponse : IDownloadContentInput
{
    public FileEventResponse(BoxWebhookSource source)
    {
        ContentId = source.Id;
        Name = source.Name;
        Path = BuildPath(source.PathCollection);
        Size = source.Size;
        Description = source.Description;
        ParentFolderId = source.Parent?.Id ?? string.Empty;
        CreatedBy = source.CreatedBy?.Name;
        ModifiedBy = source.ModifiedBy?.Name;
        CreatedAt = source.CreatedAt?.UtcDateTime;
        ModifiedAt = source.ModifiedAt?.UtcDateTime;
    }

    [Display("File ID")]
    public string ContentId { get; set; }

    [Display("File name")]
    public string Name { get; set; }

    [Display("Path")]
    public string Path { get; set; }

    [Display("Size")]
    public long? Size { get; set; }

    [Display("Description")]
    public string? Description { get; set; }

    [Display("Parent folder ID")]
    public string ParentFolderId { get; set; }

    [Display("Created by")]
    public string? CreatedBy { get; set; }

    [Display("Modified by")]
    public string? ModifiedBy { get; set; }

    [Display("Created at")]
    public DateTime? CreatedAt { get; set; }

    [Display("Modified at")]
    public DateTime? ModifiedAt { get; set; }

    private static string BuildPath(BoxWebhookPathCollection? pathCollection)
    {
        var folderNames = pathCollection?.Entries.Select(entry => entry.Name) ?? [];
        return string.Join('/', folderNames) + "/";
    }
}
