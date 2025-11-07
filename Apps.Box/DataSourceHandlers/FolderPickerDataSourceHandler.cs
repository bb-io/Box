using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Box.V2.Exceptions;
using Box.V2.Models;

namespace Apps.Box.DataSourceHandlers
{
    public class FolderPickerDataSourceHandler : BaseInvocable, IAsyncFileDataSourceItemHandler
    {
        private const string RootId = "0";
        private const string RootFolderDisplayName = "All files";

        private readonly BlackbirdBoxClient _client;

        public FolderPickerDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
        {
            _client = new BlackbirdBoxClient(
                invocationContext.AuthenticationCredentialsProviders,
                InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
        }

        public async Task<IEnumerable<FileDataItem>> GetFolderContentAsync(FolderContentDataSourceContext context, CancellationToken cancellationToken)
        {
            var folderId = string.IsNullOrWhiteSpace(context?.FolderId) ? RootId : context!.FolderId!;
            var result = new List<FileDataItem>();

            var entries = await ListFoldersInFolderByIdAsync(folderId, cancellationToken);

            foreach (var entry in entries)
            {
                result.Add(new Folder
                {
                    Id = entry.Id,
                    DisplayName = entry.Name,
                    Date = null,
                    IsSelectable = true
                });
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
            var path = new List<FolderPathItem>();

            try
            {
                var folder = await _client.FoldersManager.GetInformationAsync(id, fields: new[]
                {
                    "id", "name", "path_collection"
                });

                if (folder.PathCollection?.Entries != null && folder.PathCollection.Entries.Count > 0)
                {
                    foreach (var node in folder.PathCollection.Entries)
                    {
                        path.Add(new FolderPathItem
                        {
                            Id = node.Id,
                            DisplayName = node.Name
                        });
                    }
                }
                else
                {
                    path.Add(new FolderPathItem { Id = RootId, DisplayName = RootFolderDisplayName });
                }

                var root = path.FirstOrDefault();
                if (root != null)
                {
                    root.Id = RootId;
                    root.DisplayName = RootFolderDisplayName;
                }
            }
            catch (BoxException)
            {
                path.Clear();
                path.Add(new FolderPathItem { Id = RootId, DisplayName = RootFolderDisplayName });
            }

            return path;
        }

        private async Task<IReadOnlyList<BoxItem>> ListFoldersInFolderByIdAsync(string folderId, CancellationToken ct)
        {
            const int limit = 1000;
            var offset = 0;
            var all = new List<BoxItem>();

            while (true)
            {
                var page = await _client.FoldersManager.GetFolderItemsAsync(
                    folderId, limit, offset,
                    fields: new[] { "id", "name", "type" });

                var entries = page?.Entries ?? new List<BoxItem>();
                var onlyFolders = entries.Where(e => e.Type == "folder").ToList();

                all.AddRange(onlyFolders);
                offset += entries.Count;

                if (entries.Count < limit || ct.IsCancellationRequested)
                    break;
            }

            return all;
        }
    }
}
