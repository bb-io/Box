using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests;

public class DownloadFileRequest : IDownloadFileInput
{
    [Display("File ID"), DataSource(typeof(FileDataSourceHandler))]
    public string FileId { get; set; }
}