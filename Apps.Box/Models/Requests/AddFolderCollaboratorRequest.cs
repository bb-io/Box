using Apps.Box.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests
{
    public class AddFolderCollaboratorRequest
    {
        public string CollaboratorId { get; set; }

        public string FolderId { get; set; }

        [DataSource(typeof(RoleDataHandler))]
        public string Role { get; set; }
        
        [DataSource(typeof(StatusDataHander))]
        public string? Status { get; set; }
    }
}
