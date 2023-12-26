using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Box.Models.Requests;

public class UploadFileRequest
{
    public FileReference File { get; set; }

    [DataSource(typeof(FolderDataSourceHandler))]
    [Display("Parent folder")]
    public string ParentFolderId { get; set; }
}