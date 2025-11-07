using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class DeleteFileRequest
{
    [FileDataSource(typeof(FilePickerDataSourceHandler))]
    [Display("File ID")]
    public string FileId { get; set; }
}