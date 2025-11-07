using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class DownloadFileRequest : IDownloadFileInput
{
    [Display("File ID")]
    [FileDataSource(typeof(FilePickerDataSourceHandler))]
    public string FileId { get; set; }
}