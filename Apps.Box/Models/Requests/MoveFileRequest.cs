using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class MoveFileRequest
{
    [FileDataSource(typeof(FilePickerDataSourceHandler))]
    [Display("File")]
    public string FileId { get; set; }

    [DataSource(typeof(FolderDataSourceHandler))]
    [Display("New parent folder ID")]
    public string FolderId { get; set; }
}