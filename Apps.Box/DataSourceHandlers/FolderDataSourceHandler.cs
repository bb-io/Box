using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Box.V2.Models;

namespace Apps.Box.DataSourceHandlers;

public class FolderDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    private readonly BlackbirdBoxClient _client;
    
    public FolderDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
        _client = new BlackbirdBoxClient(invocationContext.AuthenticationCredentialsProviders);
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var folders = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(context.SearchString))
        {
            await GetTwentyFolders(folders);
            folders["0"] = "/";
        }
        else
            folders = await SearchFolders(context.SearchString);
        
        return folders;
    }

    private async Task<Dictionary<string, string>> SearchFolders(string searchString)
    {
        string GetFolderPath(BoxItem folder)
        {
            var pathFolders = folder.PathCollection.Entries.Skip(1).ToArray();
            
            if (pathFolders.Length == 0)
                return "/" + folder.Name;

            if (pathFolders.Length <= 2)
                return "/" + string.Join("/", pathFolders.Select(p => p.Name)) + "/" + folder.Name;
            
            return "/" + pathFolders[0].Name + "/.../" + pathFolders[^1].Name + "/" + folder.Name;
        }
        
        var folders = await _client.SearchManager.QueryAsync(searchString, type: "folder", limit: 20, 
            ancestorFolderIds: new[] { "0" }, contentTypes: new[] { "name" , "description" }, 
            fields: new [] { "id", "name", "path_collection" });

        return folders.Entries.ToDictionary(f => f.Id, GetFolderPath);
    }
    
    private async Task GetTwentyFolders(Dictionary<string, string> folders, int offset = 0, string folderId = "0", 
        string folderPath = "")
    {
        string GetFolderPath(BoxItem folder)
        {
            var folderPathParts = folderPath.Split("/").Skip(1).ToArray();

            if (folderPathParts.Length <= 2)
                return folderPath + "/" + folder.Name;

            return "/" + folderPathParts[0] + "/.../" + folderPathParts[^1] + "/" + folder.Name;
        }
        
        const int limit = 20;
        var items = await _client.FoldersManager.GetFolderItemsAsync(folderId, limit, offset, sort: BoxSortBy.Name.ToString(), 
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name" });

        foreach (var item in items.Entries)
        {
            if (folders.Count == limit)
                return;

            if (item.Type == "folder")
            {
                folders[item.Id] = GetFolderPath(item);
                await GetTwentyFolders(folders, offset, item.Id, folderPath + "/" + item.Name);
            }
        }

        if (items.TotalCount > limit + offset)
            await GetTwentyFolders(folders, offset + limit, folderId, folderPath);
    }
}