using System.Net.Mime;
using Apps.Box.Dtos;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Box.V2.Models;

namespace Apps.Box.Actions;

[ActionList("Files")]
public class StorageActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : BoxInvocable(invocationContext)
{
    [Action("Search files", Description = "Search for files in a folder")]
    public async Task<ListDirectoryResponse> SearchFilesInFolder([ActionParameter] SearchFilesRequest input)
    {
        if (input.SearchSubFodlers.HasValue && input.SearchSubFodlers.Value)
        {
            var files = await GetFiles(0, input.FolderId ?? "0", input.Limit ?? 1000);
            return new ListDirectoryResponse
            {
                Files = files
            };
        }

        var items = await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.GetFolderItemsAsync(
            input.FolderId ?? "0", input.Limit ?? 200, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC,
            fields: new[] { "id", "type", "name", "path_collection", "size", "description" }));
        
        if (!items.Entries.Any(i => i.Type == "file"))
        {
            return new ListDirectoryResponse { Files = new List<FileDto>() };
        }

        var folderItems = items.Entries.Where(i => i.Type == "file").Select(i => new FileDto(i, i.Id)).ToList();
        return new ListDirectoryResponse
        {
            Files = folderItems
        };
    }
    
    [Action("Download file", Description = "Download file")]
    [BlueprintActionDefinition(BlueprintAction.DownloadFile)]
    public async Task<DownloadFileResponse> DownloadFile([ActionParameter] DownloadFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException(
                "File ID is null or empty. Please check your input and try again");
        }

        var uri = await ExecuteWithErrorHandlingAsync(async () =>
            await Client.FilesManager.GetDownloadUriAsync(input.FileId));
        var fileInfo =
            await ExecuteWithErrorHandlingAsync(async () =>
                await Client.FilesManager.GetInformationAsync(input.FileId));
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
    [BlueprintActionDefinition(BlueprintAction.UploadFile)]
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

        var downloadedFile = await fileManagementClient.DownloadAsync(input!.File);

        var fileStream = new MemoryStream();
        await downloadedFile.CopyToAsync(fileStream);

        var file = await ExecuteWithErrorHandlingAsync(async () =>
            await Client.FilesManager.UploadAsync(uploadFileRequest, fileStream));
        return new(file);
    }

    [Action("Get file information", Description = "Get file information")]
    public async Task<FileDto> GetFileInformation([ActionParameter] GetFileInformationRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException(
                "File ID is null or empty. Please check your input and try again");
        }

        var file = await ExecuteWithErrorHandlingAsync(async () =>
            await Client.FilesManager.GetInformationAsync(input.FileId));
        return new FileDto(file);
    }

    [Action("Rename file", Description = "Rename file")]
    public async Task<FileDto> RenameFile([ActionParameter] RenameFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException(
                "File ID is null or empty. Please check your input and try again");
        }

        var fileUpdateRequest = new BoxFileRequest
        {
            Id = input.FileId,
            Name = input.NewFilename
        };
        var file = await ExecuteWithErrorHandlingAsync(async () =>
            await Client.FilesManager.UpdateInformationAsync(fileUpdateRequest));
        return new FileDto(file);
    }

    [Action("Delete file", Description = "Delete file")]
    public async Task DeleteFile([ActionParameter] DeleteFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException(
                "File ID is null or empty. Please check your input and try again");
        }

        await ExecuteWithErrorHandlingAsync(async () => await Client.FilesManager.DeleteAsync(input.FileId));
    }

    [Action("Copy file", Description = "Copy file")]
    public async Task CopyFile([ActionParameter] CopyFileRequest input)
    {
        if (string.IsNullOrWhiteSpace(input.FileId))
        {
            throw new PluginMisconfigurationException(
                "File ID is null or empty. Please check your input and try again");
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

    private async Task<List<FileDto>> GetFiles(int offset = 0, string folderId = "0", int limit = 1000)
    {
        var files = new List<FileDto>();

        var items = await ExecuteWithErrorHandlingAsync(async () => await Client.FoldersManager.GetFolderItemsAsync(
            folderId, limit, 0, sort: BoxSortBy.Name.ToString(),
            direction: BoxSortDirection.DESC,
            fields: new[] { "id", "type", "name", "path_collection", "size", "description", "parent" }));
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
}