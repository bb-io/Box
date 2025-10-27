using Blackbird.Applications.Sdk.Common;
using Box.V2.Models;

namespace Apps.Box.Dtos;

public class BasicFolderDto
{
    public BasicFolderDto(BoxItem folder)
    {
        FolderId = folder.Id;
        FolderName = folder.Name;
        CreatedBy = folder.CreatedBy?.Name;
        ModifiedBy = folder.ModifiedBy?.Name;
        var entries = folder.PathCollection?.Entries?
           .Select(e => e.Name)
           .Where(n => !string.IsNullOrWhiteSpace(n))
           .ToList() ?? new List<string>();

        entries.Add(folder.Name);

        Path = "/" + string.Join("/", entries);
    }

    [Display("Folder ID")]
    public string FolderId { get; set; }

    [Display("Folder name")]
    public string FolderName { get; set; }

    [Display("Created by")]
    public string? CreatedBy { get; set; }

    [Display("Modified by")]
    public string? ModifiedBy { get; set; }

    [Display("Path")]
    public string Path { get; set; }
}