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

namespace Apps.Box;

[ActionList]
public class Actions : BaseInvocable
{
    private IEnumerable<AuthenticationCredentialsProvider> Creds =>
        InvocationContext.AuthenticationCredentialsProviders;

    private readonly IFileManagementClient _fileManagementClient;

    public Actions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Search files in folder", Description = "Search for files in a folder")]
    public async Task<ListDirectoryResponse> ListDirectory([ActionParameter] SearchFilesRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());

        if (input.SearchSubFodlers.HasValue && input.SearchSubFodlers.Value)
        {
            var files = await GetFiles(0, input.FolderId ?? "0", input.Limit ?? 1000);
            return new ListDirectoryResponse
            {
                Files = files
            };
        }

        var items = await client.FoldersManager.GetFolderItemsAsync(input.FolderId ?? "0", input.Limit ?? 200, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name", "path_collection", "size", "description" });
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
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());

        var items = await client.FoldersManager.GetFolderItemsAsync(folderId, limit, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name", "path_collection", "size", "description", "parent" });
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
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        var file = await client.FilesManager.GetInformationAsync(input.FileId);
        return new FileDto(file);
    }

    [Action("Rename file", Description = "Rename file")]
    public async Task<FileDto> RenameFile([ActionParameter] RenameFileRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        var fileUpdateRequest = new BoxFileRequest
        {
            Id = input.FileId,
            Name = input.NewFilename
        };
        var file = await client.FilesManager.UpdateInformationAsync(fileUpdateRequest);
        return new FileDto(file);
    }

    [Action("Download file", Description = "Download file")]
    public async Task<DownloadFileResponse> DownloadFile([ActionParameter] DownloadFileRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        var uri = await client.FilesManager.GetDownloadUriAsync(input.FileId);
        var fileInfo = await client.FilesManager.GetInformationAsync(input.FileId);
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
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        var uploadFileRequest = new BoxFileRequest
        {
            Name = input.File.Name,
            Parent = new BoxRequestEntity
            {
                Id = input.ParentFolderId
            }
        };

        var downloadedFile = await _fileManagementClient.DownloadAsync(input.File);
        
        var fileStream = new MemoryStream();
        await downloadedFile.CopyToAsync(fileStream); 
        
        var file = await client.FilesManager.UploadAsync(uploadFileRequest, fileStream);
        return new(file);
    }

    [Action("Delete file", Description = "Delete file")]
    public async Task DeleteFile([ActionParameter] DeleteFileRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        await client.FilesManager.DeleteAsync(input.FileId);
    }

    [Action("Copy file", Description = "Copy file")]
    public async Task CopyFile([ActionParameter] CopyFileRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        await client.FilesManager.CopyAsync(new BoxFileRequest 
        {
            Name = input.NewName,
            Parent = new BoxRequestEntity
            {
                Id = input.ParentFolderId
            },
            Id = input.FileId
        });

        if (input.RemoveOriginal.HasValue && input.RemoveOriginal.Value)
        {
            await client.FilesManager.DeleteAsync(input.FileId);
        }
    }

    [Action("Create folder", Description = "Create folder")]
    public async Task<FolderDto> CreateDirectory([ActionParameter] CreateFolderRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        try
        {
            var folderRequest = new BoxFolderRequest
            {
                Name = input.FolderName,
                Parent = new BoxRequestEntity
                {
                    Id = input.ParentFolderId
                }
            };
            var folder = await client.FoldersManager.CreateAsync(folderRequest);
            return new FolderDto(folder);

        }
        catch (Exception x)
        {
            if (x.Message.Contains("already exists"))

            {
                var folderID = Regex.Match(x.Message, "\\?\"id\\?\":\\?\"(.*?)\\?\"").Groups[1].Value;
                var folder = await client.FoldersManager.GetInformationAsync(folderID, fields: new[] { "id", "type", "name", "parent" });
                return new FolderDto(folder);
            }

            else
            {
                throw x;
            }
        }

    }

    [Action("Delete folder", Description = "Delete folder")]
    public async Task DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        await client.FoldersManager.DeleteAsync(input.FolderId);
    }

    [Action("Add collaborator to folder", Description = "Add collaborator to folder")]
    public async Task<CollaborationDto> AddCollaboratorToFolder([ActionParameter] AddFolderCollaboratorRequest input)
    {
        var client = new BlackbirdBoxClient(Creds, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
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
        var collaboration = await client.CollaborationsManager.AddCollaborationAsync(addCollaboratorRequest,
            notify: input.NotifyCollaborator);
        return new CollaborationDto(collaboration);
    }

    //[Action("Debug", Description = "Search for files in a folder")]
    //public async Task<DebugResponse> Debug()
    //{     
    //    return new DebugResponse
    //    {
    //        AccessToken = Creds.First(p => p.KeyName == "access_token").Value,
    //    };
    //}
}