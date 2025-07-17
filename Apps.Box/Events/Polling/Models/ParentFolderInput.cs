using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Events.Polling.Models;

public class ParentFolderInput
{
    [Display("Parent folder ID"), DataSource(typeof(FolderDataSourceHandler))]
    public string? FolderId { get; set; }
}