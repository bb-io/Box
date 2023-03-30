using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Apps.Box.Models.Requests;
using Apps.Box.Models.Responses;
using Apps.Box.Dtos;
using Box.V2.Auth;
using Box.V2.Config;
using Box.V2;
using Box.V2.Models;

namespace Apps.Box
{
    [ActionList]
    public class Actions
    {
        [Action("List directory", Description = "List specified directory")]
        public ListDirectoryResponse ListDirectory(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] ListDirectoryRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            var items = client.FoldersManager.GetFolderItemsAsync(input.FolderId, input.Limit).Result.Entries;
            var folderItems = items.Select(i => new DirectoryItemDto()
            {
                Name = i.Name,
            }).ToList();
            return new ListDirectoryResponse()
            {
                DirectoriesItems = folderItems
            };
        }

        [Action("Get file information", Description = "Get file information")]
        public GetFileInformationResponse GetFileInformation(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] GetFileInformationRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            var fileInfo = client.FilesManager.GetInformationAsync(input.FileId).Result;
            return new GetFileInformationResponse()
            {
                Path = string.Join('/', fileInfo.PathCollection.Entries.Select(p => p.Name)),
                Size = (long)fileInfo.Size
            };
        }

        [Action("Rename file", Description = "Rename file")]
        public void RenameFile(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] RenameFileRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            var fileUpdateRequest = new BoxFileRequest()
            {
                Id = input.FileId,
                Name = input.NewFilename
            };
            client.FilesManager.UpdateInformationAsync(fileUpdateRequest).Wait();
        }

        [Action("Download file", Description = "Download file by id")]
        public DownloadFileResponse DownloadFile(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] DownloadFileRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            var fileStream = client.FilesManager.DownloadAsync(input.FileId).Result;
            using(var memStream = new MemoryStream())
            {
                fileStream.CopyTo(memStream);
                return new DownloadFileResponse()
                {
                    File = memStream.ToArray(),
                };
            }
        }

        [Action("Upload file", Description = "Upload file")]
        public void UploadFile(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] UploadFileRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            var uploadFileRequest = new BoxFileRequest()
            {
                Name = input.FileName,
                Parent = new BoxRequestEntity()
                {
                    Id = input.ParentFolderId
                }
            };
            using (var stream = new MemoryStream(input.File))
            {
                client.FilesManager.UploadAsync(uploadFileRequest, stream).Wait();
            }    
        }

        [Action("Delete file", Description = "Delete file by id")]
        public void DeleteFile(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] DeleteFileRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            client.FilesManager.DeleteAsync(input.FileId).Wait();
        }

        [Action("Create folder", Description = "Create folder")]
        public void CreateDirectory(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] CreateFolderRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
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
        public void DeleteDirectory(string clientId, string clientSecret, AuthenticationCredentialsProvider authenticationCredentialsProvider,
           [ActionParameter] DeleteDirectoryRequest input)
        {
            var client = GetBoxClient(clientId, clientSecret, authenticationCredentialsProvider.Value);
            client.FoldersManager.DeleteAsync(input.FolderId).Wait();
        }

        private BoxClient GetBoxClient(string clientId, string clientSecret, string devToken)
        {
            var config = new BoxConfigBuilder(clientId, clientSecret, new Uri("http://localhost")).Build();
            var session = new OAuthSession(devToken, "N/A", 3600, "bearer");
            var client = new BoxClient(config, session);
            return client;
        }
    }
}
