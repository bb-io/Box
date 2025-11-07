using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class CreateFolderRequest
{
    [Display("Folder name")]
    public string FolderName { get; set; }

    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    [Display("Parent folder ID")]
    public string ParentFolderId { get; set; }
}