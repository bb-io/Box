using Apps.Box.Dtos;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Box.V2.Models;

namespace Apps.Box.Actions;

[ActionList("Folders")]
public class FolderActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BoxInvocable(invocationContext)
{
    [Action("Create folder", Description = "Create folder")]
    public async Task<string> CreateFolder([ActionParameter] CreateFolderRequest input)
    {
        var items = await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.GetFolderItemsAsync(
            input.ParentFolderId, 300, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name" }));

        var folders = items.Entries.Where(i => i.Type == "folder").ToList();
        if (folders.Any(x => x.Name == input.FolderName))
        {
            return folders.FirstOrDefault(x => x.Name == input.FolderName).Id;
        }

        var folderRequest = new BoxFolderRequest
        {
            Name = input.FolderName,
            Parent = new BoxRequestEntity
            {
                Id = input.ParentFolderId
            }
        };
        var folder =
            await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.CreateAsync(folderRequest));
        return folder.Id;
    }

    [Action("Delete folder", Description = "Delete folder")]
    public async Task DeleteFolder([ActionParameter] DeleteDirectoryRequest input)
    {
        await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.DeleteAsync(input.FolderId));
    }

    [Action("Add collaborator to folder", Description = "Add collaborator to folder")]
    public async Task<CollaborationDto> AddCollaboratorToFolder([ActionParameter] AddFolderCollaboratorRequest input)
    {
        var addCollaboratorRequest = new BoxCollaborationRequest
        {
            AccessibleBy = new BoxCollaborationUserRequest
            {
                Login = input.CollaboratorEmail,
                Type = BoxType.user
            },
            Item = new BoxRequestEntity
            {
                Id = input.FolderId,
                Type = BoxType.folder
            },
            Role = input.Role
        };
        
        var collaboration = await ExecuteWithErrorHandlingAsync(async () =>
            await Client.CollaborationsManager.AddCollaborationAsync(addCollaboratorRequest,
                notify: input.NotifyCollaborator));
        return new CollaborationDto(collaboration);
    }

    [Action("Search folders", Description = "List folders in a parent folder")]
    public async Task<SearchFoldersResponse> SearchFoldersInFolder([ActionParameter] SearchFilesRequest input)
    {
        var result = new List<BasicFolderDto>();

        async Task<List<BasicFolderDto>> GetFolders(string folderId, int offset = 0, int limit = 200, int currentDepth = 0)
        {
            var folders = new List<BasicFolderDto>();

            var items = await ExecuteWithErrorHandlingAsync(async () =>
                await Client.FoldersManager.GetFolderItemsAsync(
                    folderId,
                    limit,
                    offset,
                    sort: BoxSortBy.Name.ToString(),
                    direction: BoxSortDirection.DESC,
                    fields: new[] { "id", "type", "name", "created_by", "modified_by", "path_collection" }
                )
            );

            var foundFolders = items.Entries
                .Where(i => i.Type == "folder")
                .Select(i => new BasicFolderDto(i))
                .ToList();

            folders.AddRange(foundFolders);

            if (input.SearchSubFodlers == true && (!input.MaxDepthLevel.HasValue || currentDepth < input.MaxDepthLevel.Value))
            {
                foreach (var folder in foundFolders)
                {
                    var subFolders = await GetFolders(folder.FolderId, 0, limit, currentDepth + 1);
                    folders.AddRange(subFolders);
                }
            }

            if (items.TotalCount > offset + limit)
            {
                var moreFolders = await GetFolders(folderId, offset + limit, limit);
                folders.AddRange(moreFolders);
            }

            return folders;
        }
        result.AddRange(await GetFolders(input.FolderId ?? "0", 0, input.Limit ?? 200, 0));

        return new SearchFoldersResponse
        {
            Folders = result
        };
    }
}