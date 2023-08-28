using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Apps.Box.Dtos;
using Box.V2.Models;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Box
{
    [ActionList]
    public class Actions
    {
        [Action("List directory", Description = "List specified directory")]
        public async Task<ListDirectoryResponse> ListDirectory(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] ListDirectoryRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            var items = await client.FoldersManager.GetFolderItemsAsync(input.FolderId, input.Limit);
            var folderItems = items.Entries.Select(i => new DirectoryItemDto()
            {
                Name = i.Name,
            }).ToList();
            return new ListDirectoryResponse()
            {
                DirectoriesItems = folderItems
            };
        }

        [Action("Get file information", Description = "Get file information")]
        public async Task<GetFileInformationResponse> GetFileInformation(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] GetFileInformationRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            var fileInfo = await client.FilesManager.GetInformationAsync(input.FileId);
            
            return new GetFileInformationResponse()
            {
                Path = string.Join('/', fileInfo.PathCollection.Entries.Select(p => p.Name)),
                Size = (long)fileInfo.Size
            };
        }

        [Action("Rename file", Description = "Rename file")]
        public void RenameFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] RenameFileRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            var fileUpdateRequest = new BoxFileRequest()
            {
                Id = input.FileId,
                Name = input.NewFilename
            };
            client.FilesManager.UpdateInformationAsync(fileUpdateRequest).Wait();
        }

        [Action("Download file", Description = "Download file by id")]
        public async Task<DownloadFileResponse> DownloadFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] DownloadFileRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
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
        public void UploadFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] UploadFileRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
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
                client.FilesManager.UploadAsync(uploadFileRequest, stream).Wait();
            }    
        }

        [Action("Delete file", Description = "Delete file by id")]
        public void DeleteFile(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] DeleteFileRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            client.FilesManager.DeleteAsync(input.FileId).Wait();
        }

        [Action("Create folder", Description = "Create folder")]
        public void CreateDirectory(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] CreateFolderRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            var folderRequest = new BoxFolderRequest()
            {
                Name = input.FolderName,
                Parent = new BoxRequestEntity()
                {
                    Id = input.ParentFolderId
                }
            };
            client.FoldersManager.CreateAsync(folderRequest).Wait();
        }

        [Action("Delete directory", Description = "Delete directory")]
        public void DeleteDirectory(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] DeleteDirectoryRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            client.FoldersManager.DeleteAsync(input.FolderId).Wait();
        }

        [Action("Add collaborator to folder", Description = "Add collaborator to folder")]
        public void AddCollaboratorToFolder(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
           [ActionParameter] AddFolderCollaboratorRequest input)
        {
            var client = new BlackbirdBoxClient(authenticationCredentialsProviders);
            var addCollaboratorRequest = new BoxCollaborationRequest
            {
                AccessibleBy = new BoxCollaborationUserRequest()
                {
                    Id = input.CollaboratorId,
                    Type = BoxType.user
                },
                Item = new BoxRequestEntity()
                {
                    Id = input.FolderId,
                    Type = BoxType.folder
                },
                Role = input.Role,
                Status = input.Status
            };
            client.CollaborationsManager.AddCollaborationAsync(addCollaboratorRequest).Wait();
        }
    }
}
