using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Box.V2.Exceptions;
using Box.V2.Models;

namespace Apps.Box.DataSourceHandlers
{
    public class FilePickerDataSourceHandler : BaseInvocable, IAsyncFileDataSourceItemHandler
    {
        private readonly BlackbirdBoxClient _client;
        private const string RootId = "0";
        private const string RootFolderDisplayName = "All files";

        public FilePickerDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
        {
            _client = new BlackbirdBoxClient(invocationContext.AuthenticationCredentialsProviders, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        }

        public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken cancellationToken)
        {
            var folderId = string.IsNullOrWhiteSpace(context?.FolderId) ? RootId : context!.FolderId!;
            var result = new List<FileDataItem>();

            var entries = await ListItemsInFolderByIdAsync(folderId, cancellationToken);

            foreach (var entry in entries)
            {
                switch (entry.Type)
                {
                    case "folder":
                        result.Add(new Folder
                        {
                            Id = entry.Id,
                            DisplayName = entry.Name,
                            Date = null,
                            IsSelectable = false
                        });
                        break;

                    case "file":
                        BoxFile? fileInfo = null;
                        try
                        {
                            fileInfo = await _client.FilesManager.GetInformationAsync(entry.Id, fields: new[]
                            {
                                "id", "name", "size", "modified_at"
                            });
                        }
                        catch (BoxException)
                        {
                        }

                        result.Add(new Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.File
                        {
                            Id = entry.Id,
                            DisplayName = entry.Name,
                            Size = fileInfo?.Size,
                            IsSelectable = true
                        });
                        break;
                }
            }

            return result;
        }

        public async Task<IEnumerable<FolderPathItem>> GetFolderPathAsync(FolderPathDataSourceContext context, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(context?.FileDataItemId))
            {
                return new List<FolderPathItem>
                {
                    new FolderPathItem { Id = RootId, DisplayName = RootFolderDisplayName }
                };
            }

            var id = context!.FileDataItemId!;
            var pathItems = new List<FolderPathItem>();

            BoxItem? target = null;
            try
            {
                var file = await _client.FilesManager.GetInformationAsync(id, fields: new[]
                {
                    "id", "name", "path_collection"
                });

                target = file;
                if (file.PathCollection?.Entries != null)
                {
                    foreach (var folder in file.PathCollection.Entries)
                    {
                        pathItems.Add(new FolderPathItem
                        {
                            Id = folder.Id,
                            DisplayName = folder.Name
                        });
                    }
                }
            }
            catch (BoxException)
            {
            }

            if (target == null)
            {
                try
                {
                    var folder = await _client.FoldersManager.GetInformationAsync(id, fields: new[]
                    {
                        "id", "name", "path_collection"
                    });

                    if (folder.PathCollection?.Entries != null)
                    {
                        foreach (var pf in folder.PathCollection.Entries)
                        {
                            pathItems.Add(new FolderPathItem
                            {
                                Id = pf.Id,
                                DisplayName = pf.Name
                            });
                        }
                    }
                }
                catch
                {
                }
            }

            if (pathItems.Count == 0)
            {
                pathItems.Add(new FolderPathItem { Id = RootId, DisplayName = RootFolderDisplayName });
            }
            else
            {
                var root = pathItems.First();
                root.Id = RootId;
                root.DisplayName = RootFolderDisplayName;
            }

            return pathItems;
        }

        private async Task<IReadOnlyList<BoxItem>> ListItemsInFolderByIdAsync(string folderId, CancellationToken ct)
        {
            const int limit = 1000;
            var offset = 0;
            var all = new List<BoxItem>();

            while (true)
            {
                var page = await _client.FoldersManager.GetFolderItemsAsync(
                    folderId, limit, offset,
                    fields: new[] { "id", "name", "type" });

                if (page?.Entries == null || page.Entries.Count == 0)
                    break;

                all.AddRange(page.Entries);
                offset += page.Entries.Count;

                if (page.Entries.Count < limit)
                    break;

                if (ct.IsCancellationRequested)
                    break;
            }

            return all;
        }
    }
}
