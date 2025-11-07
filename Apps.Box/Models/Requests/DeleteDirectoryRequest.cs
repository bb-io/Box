using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class DeleteDirectoryRequest
{
    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    [Display("Folder ID")]
    public string FolderId { get; set; }
}