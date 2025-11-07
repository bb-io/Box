using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class RenameFileRequest
{
    [FileDataSource(typeof(FilePickerDataSourceHandler))]
    [Display("File")]
    public string FileId { get; set; }

    [Display("New filename")]
    public string NewFilename { get; set; }
}