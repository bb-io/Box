using Apps.Box.DataSourceHandlers;
using Apps.Box.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests;

public class AddFolderCollaboratorRequest
{
    [Display("Collaborator's email")]
    public string CollaboratorEmail { get; set; }
        
    [Display("Notify collaborator")]
    public bool NotifyCollaborator { get; set; }
        
    [DataSource(typeof(FolderDataSourceHandler))]
    [Display("Folder")]
    public string FolderId { get; set; }

    [DataSource(typeof(RoleDataHandler))]
    public string Role { get; set; }
}