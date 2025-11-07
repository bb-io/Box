using Apps.Box.DataSourceHandlers;
using Apps.Box.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class AddFolderCollaboratorRequest
{
    [Display("Collaborator's email")]
    public string CollaboratorEmail { get; set; }
        
    [Display("Notify collaborator")]
    public bool NotifyCollaborator { get; set; }

    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    [Display("Folder ID")]
    public string FolderId { get; set; }

    [StaticDataSource(typeof(RoleDataHandler))]
    public string Role { get; set; }
}