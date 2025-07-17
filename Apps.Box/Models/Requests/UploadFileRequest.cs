using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Box.Models.Requests;

public class UploadFileRequest : IUploadFileInput
{
    public FileReference File { get; set; }

    [Display("Parent folder"), DataSource(typeof(FolderDataSourceHandler))]
    public string ParentFolderId { get; set; }
}