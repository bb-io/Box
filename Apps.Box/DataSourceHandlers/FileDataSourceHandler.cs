using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Box.V2.Models;

namespace Apps.Box.DataSourceHandlers;

public class FileDataSourceHandler : BaseInvocable, IAsyncDataSourceHandler
{
    private readonly BlackbirdBoxClient _client;
    
    public FileDataSourceHandler(InvocationContext invocationContext) : base(invocationContext)
    {
       _client = new BlackbirdBoxClient(invocationContext.AuthenticationCredentialsProviders, InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var files = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(context.SearchString))
            await GetTwentyFiles(files);
        else
            files = await SearchFiles(context.SearchString);

        return files;
    }

    private async Task<Dictionary<string, string>> SearchFiles(string searchString)
    {
        string GetFilePath(BoxItem file)
        {
            var pathFolders = file.PathCollection.Entries.Skip(1).ToArray();

            if (pathFolders.Length <= 2)
                return string.Join("/", pathFolders.Select(p => p.Name)) + "/" + file.Name;
            
            return pathFolders[0].Name + "/.../" + pathFolders[^1].Name + "/" + file.Name;
        }
        
        var files = await _client.SearchManager.QueryAsync(searchString, type: "file", limit: 20, ancestorFolderIds: new[] { "0" }, 
            contentTypes: new[] { "name" , "description" }, fields: new [] { "id", "name", "path_collection" });

        return files.Entries.ToDictionary(f => f.Id, GetFilePath);
    }
    
    private async Task GetTwentyFiles(Dictionary<string, string> files, int offset = 0, string folderId = "0", 
        string folderPath = "")
    {
        string GetFilePath(BoxItem file)
        {
            var folderPathParts = folderPath.Split("/").ToArray();
            
            if (folderPathParts.Length == 1 && folderPathParts[0] == "")
                return file.Name;

            if (folderPathParts.Length <= 2)
                return folderPath + "/" + file.Name;

            return folderPathParts[0] + "/.../" + folderPathParts[^1] + "/" + file.Name;
        }
        
        const int limit = 20;
        var items = await _client.FoldersManager.GetFolderItemsAsync(folderId, limit, offset, sort: BoxSortBy.Name.ToString(), 
            direction: BoxSortDirection.DESC, fields: new[] { "id", "type", "name" });

        foreach (var item in items.Entries)
        {
            if (files.Count == limit)
                return;
            
            if (item.Type == "file")
                files[item.Id] = GetFilePath(item);
                
            else if (item.Type == "folder")
            {
                var subfolderPath = folderPath == "" ? item.Name : folderPath + "/" + item.Name;
                await GetTwentyFiles(files, offset, item.Id, subfolderPath);
            }
        }

        if (items.TotalCount > limit + offset)
            await GetTwentyFiles(files, offset + limit, folderId, folderPath);
    }
}