using System.Net.Mime;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Apps.Box.Dtos;
using Box.V2.Models;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Common.Files;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Generic;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Box;

[ActionList]
public class Actions : BoxInvocable
{

    private readonly IFileManagementClient _fileManagementClient;

    public Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Search files in folder", Description = "Search for files in a folder")]
    public async Task<ListDirectoryResponse> ListDirectory([ActionParameter] SearchFilesRequest input)
    {
        if (input.SearchSubFodlers.HasValue && input.SearchSubFodlers.Value)
        {
            var files = await GetFiles(0, input.FolderId ?? "0", input.Limit ?? 1000);
            return new ListDirectoryResponse
            {
                Files = files
            };
        }

        var items = await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.GetFolderItemsAsync(input.FolderId ?? "0", input.Limit ?? 200, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name", "path_collection", "size", "description" }));
        if (!items.Entries.Any(i => i.Type == "file"))
        { return new ListDirectoryResponse { Files = new List<FileDto>()}; }
        var folderItems = items.Entries.Where(i => i.Type == "file").Select(i => new FileDto(i, i.Id)).ToList();

        return new ListDirectoryResponse
        {
            Files = folderItems
        };
    }

    private async Task<List<FileDto>> GetFiles(int offset = 0, string folderId = "0", int limit = 1000)
    {
        var files = new List<FileDto>();

        var items = await ExecuteWithErrorHandlingAsync(async ()=> await Client.FoldersManager.GetFolderItemsAsync(folderId, limit, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name", "path_collection", "size", "description", "parent" }));
        var foundFiles = items.Entries.Where(i => i.Type == "file").Select(i => new FileDto(i, i.Id)).ToList();

        foreach (var item in items.Entries)
        {
            if (files.Count == limit)
                return files;

            if (item.Type == "file")
                files.Add(new FileDto(item, item.Id));

            else if (item.Type == "folder")
            {
                var newFiles = await GetFiles(offset, item.Id);
                files.AddRange(newFiles);
            }
        }

        if (items.TotalCount > limit + offset)
        {
            var newFiles = await GetFiles(offset + limit, folderId);
            files.AddRange(newFiles);
        }

        return files;

    }

    [Action("Get file information", Description = "Get file information")]
    public async Task<FileDto> GetFileInformation([ActionParameter] GetFileInformationRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException("File ID is null or empty. Please check your input and try again");
        }

        var file = await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.GetInformationAsync(input.FileId));
        return new FileDto(file);
    }

    [Action("Rename file", Description = "Rename file")]
    public async Task<FileDto> RenameFile([ActionParameter] RenameFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException("File ID is null or empty. Please check your input and try again");
        }

        var fileUpdateRequest = new BoxFileRequest
        {
            Id = input.FileId,
            Name = input.NewFilename
        };
        var file = await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.UpdateInformationAsync(fileUpdateRequest));
        return new FileDto(file);
    }

    [Action("Download file", Description = "Download file")]
    public async Task<DownloadFileResponse> DownloadFile([ActionParameter] DownloadFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException("File ID is null or empty. Please check your input and try again");
        }

        var uri = await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.GetDownloadUriAsync(input.FileId));
        var fileInfo = await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.GetInformationAsync(input.FileId));
        var fileName = fileInfo.Name;

        if (!MimeTypes.TryGetMimeType(fileName, out var contentType))
            contentType = MediaTypeNames.Application.Octet;

        var fileRequest = new HttpRequestMessage(HttpMethod.Get, uri);
        var file = new FileReference(fileRequest, fileName, contentType);

        return new()
        {
            File = file
        };
    }

    [Action("Upload file", Description = "Upload file")]
    public async Task<FileDto> UploadFile([ActionParameter] UploadFileRequest input)
    {
        if (input.File is null)
        {
            throw new PluginMisconfigurationException("File is null or empty. Please check your input and try again");
        }

        var uploadFileRequest = new BoxFileRequest
        {
            Name = input!.File!.Name,
            Parent = new BoxRequestEntity
            {
                Id = input!.ParentFolderId
            }
        };

        var downloadedFile = await _fileManagementClient.DownloadAsync(input!.File);
        
        var fileStream = new MemoryStream();
        await downloadedFile.CopyToAsync(fileStream); 
        
        var file = await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.UploadAsync(uploadFileRequest, fileStream));
        return new(file);
    }

    [Action("Delete file", Description = "Delete file")]
    public async Task DeleteFile([ActionParameter] DeleteFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException("File ID is null or empty. Please check your input and try again");
        }
        await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.DeleteAsync(input.FileId));
    }

    [Action("Copy file", Description = "Copy file")]
    public async Task CopyFile([ActionParameter] CopyFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException("File ID is null or empty. Please check your input and try again");
        }

        await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.CopyAsync(new BoxFileRequest 
        {
            Name = input.NewName,
            Parent = new BoxRequestEntity
            {
                Id = input.ParentFolderId
            },
            Id = input.FileId
        }));

        if (input.RemoveOriginal.HasValue && input.RemoveOriginal.Value)
        {
            await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.DeleteAsync(input.FileId));
        }
    }

    [Action("Create folder", Description = "Create folder")]
    public async Task<string> CreateDirectory([ActionParameter] CreateFolderRequest input)
    {
        
        var items = await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.GetFolderItemsAsync(input.ParentFolderId, 300, 0, sort: BoxSortBy.Name.ToString(),
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
        var folder = await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.CreateAsync(folderRequest));
        return folder.Id;

    }

    [Action("Delete folder", Description = "Delete folder")]
    public async Task DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
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
        var collaboration = await ExecuteWithErrorHandlingAsync(async () => await Client.CollaborationsManager.AddCollaborationAsync(addCollaboratorRequest,
            notify: input.NotifyCollaborator));
        return new CollaborationDto(collaboration);
    }

    [Action("Debug", Description = "Search for files in a folder")]
    public async Task<DebugResponse> Debug()
    {
        return new DebugResponse
        {
            AccessToken = Creds.First(p => p.KeyName == "access_token").Value,
        };
    }
}