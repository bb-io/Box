using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests;

public class CreateFolderRequest
{
    [Display("Folder name")]
    public string FolderName { get; set; }

    [DataSource(typeof(FolderDataSourceHandler))]
    [Display("Parent folder ID")]
    public string ParentFolderId { get; set; }
}