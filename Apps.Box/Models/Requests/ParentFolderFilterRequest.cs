using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class ParentFolderFilterRequest
{
    [Display("Parent folder ID",
        Description = "Trigger only for files placed directly in this folder.")]
    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    public string ParentFolderId { get; set; } = string.Empty;
}
