using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Events.Polling.Models;

public class ParentFolderInput
{
    [Display("Parent folder ID")]
    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    public string? FolderId { get; set; }
}