using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests;

public class SearchFilesRequest
{
    [DataSource(typeof(FolderDataSourceHandler))]
    [Display("Parent folder ID", Description = "If not set, it will use the root level folder")]
    public string? FolderId { get; set; }

    [Display("Limit", Description = "The maximum amount of files to return, defaults to 200 (max).")]
    public int? Limit { get; set; }

    [Display("Include subfolders?", Description = "If set to true, it will recursively search through all folders")]
    public bool? SearchSubFodlers { get; set; }

    [Display("Max depth level", Description = "How many levels deep to search under the parent folder when including subfolders. For example, 1 = only direct children, 2 = children + grandchildren. Ignored if 'Include subfolders?' is false.")]
    public int? MaxDepthLevel { get; set; }
}