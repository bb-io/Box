using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests;

public class MoveFileRequest
{
    [DataSource(typeof(FileDataSourceHandler))]
    [Display("File")]
    public string FileId { get; set; }

    [DataSource(typeof(FolderDataSourceHandler))]
    [Display("New parent folder ID")]
    public string FolderId { get; set; }
}