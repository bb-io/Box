﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Apps.Box.Dtos;
using Box.V2.Models;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Box
{
    [ActionList]
    public class Actions : BaseInvocable
    {
        private IEnumerable<AuthenticationCredentialsProvider> Creds =>
            InvocationContext.AuthenticationCredentialsProviders;

        public Actions(InvocationContext invocationContext) : base(invocationContext) { }
        
        [Action("List directory", Description = "List specified directory")]
        public async Task<ListDirectoryResponse> ListDirectory([ActionParameter] ListDirectoryRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
            var items = await client.FoldersManager.GetFolderItemsAsync(input.FolderId, input.Limit);
            var folderItems = items.Entries.Select(i => new DirectoryItemDto { Name = i.Name }).ToList();
            
            return new ListDirectoryResponse
            {
                DirectoriesItems = folderItems
            };
        }

        [Action("Get file information", Description = "Get file information")]
        public async Task<FileDto> GetFileInformation([ActionParameter] GetFileInformationRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
            var file = await client.FilesManager.GetInformationAsync(input.FileId);
            return new FileDto(file);
        }

        [Action("Rename file", Description = "Rename file")]
        public async Task<FileDto> RenameFile([ActionParameter] RenameFileRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
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
            var client = new BlackbirdBoxClient(Creds);
            var fileStream = await client.FilesManager.DownloadAsync(input.FileId);
            var fileInfo = await client.FilesManager.GetInformationAsync(input.FileId);
            var bytes = await fileStream.GetByteData();
            var filename = fileInfo.Name;
            
            if (!MimeTypes.TryGetMimeType(filename, out var contentType))
                contentType = "application/octet-stream";
            
            return new DownloadFileResponse
            {
                File = new File(bytes)
                {
                    Name = filename,
                    ContentType = contentType
                }
            };
        }

        [Action("Upload file", Description = "Upload file")]
        public async Task<FileDto> UploadFile([ActionParameter] UploadFileRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
            var uploadFileRequest = new BoxFileRequest
            {
                Name = input.File.Name,
                Parent = new BoxRequestEntity
                {
                    Id = input.ParentFolderId
                }
            };

            using (var stream = new MemoryStream(input.File.Bytes))
            {
                var file = await client.FilesManager.UploadAsync(uploadFileRequest, stream);
                return new FileDto(file);
            }    
        }

        [Action("Delete file", Description = "Delete file")]
        public async Task DeleteFile([ActionParameter] DeleteFileRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
            await client.FilesManager.DeleteAsync(input.FileId);
        }

        [Action("Create folder", Description = "Create folder")]
        public async Task<FolderDto> CreateDirectory([ActionParameter] CreateFolderRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
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

        [Action("Delete directory", Description = "Delete directory")]
        public async Task DeleteDirectory([ActionParameter] DeleteDirectoryRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
            await client.FoldersManager.DeleteAsync(input.FolderId);
        }

        [Action("Add collaborator to folder", Description = "Add collaborator to folder")]
        public async Task<CollaborationDto> AddCollaboratorToFolder([ActionParameter] AddFolderCollaboratorRequest input)
        {
            var client = new BlackbirdBoxClient(Creds);
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
    }
}
