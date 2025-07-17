using Apps.Box.Events.Polling.Models;
using Apps.Box.Events.Polling.Models.Memory;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Box.V2.Models;

namespace Apps.Box.Events.Polling;

[PollingEventList]
public class PollingList : BaseInvocable
{
    private readonly BlackbirdBoxClient _client;

    public PollingList(InvocationContext invocationContext) : base(invocationContext)
    {
        _client = new BlackbirdBoxClient(InvocationContext.AuthenticationCredentialsProviders,
            InvocationContext.UriInfo.AuthorizationCodeRedirectUri.ToString());
    }

    [PollingEvent("On files created or updated", "On any files created or updated")]
    [BlueprintEventDefinition(BlueprintEvent.FilesCreatedOrUpdated)]
    public async Task<PollingEventResponse<DateMemory, ListFilesResponse>> OnFilesAddedOrUpdated(PollingEventRequest<DateMemory> request,
        [PollingEventParameter] ParentFolderInput parentFolder)
    {
        if (request.Memory == null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTimeOffset.UtcNow
                }
            };
        }

        var changedItems = (await GetAllFiles())
            .Where(x => x.CreatedAt > request.Memory.LastInteractionDate ||
                        x.ModifiedAt > request.Memory.LastInteractionDate)
            .ToArray();

        if (changedItems.Length == 0)
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTimeOffset.UtcNow
                }
            };
        
        changedItems = changedItems
            .Where(x => parentFolder.FolderId == null || x.PathCollection?.Entries.Any(e => e.Id == parentFolder.FolderId) == true)
            .ToArray();

        return new()
        {
            FlyBird = true,
            Memory = new()
            {
                LastInteractionDate = DateTimeOffset.UtcNow
            },
            Result = new()
            {
                Files = changedItems.Select(x => new PollingFileResponse(x))
            }
        };
    }

    [PollingEvent("On files deleted", "On any files deleted")]
    public async Task<PollingEventResponse<DateMemory, ListFilesResponse>> OnFilesDeleted(
        PollingEventRequest<DateMemory> request
    )
    {
        if (request.Memory == null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTimeOffset.UtcNow
                }
            };
        }

        var trashedFiles = await _client.FoldersManager.GetTrashItemsAsync(limit: 1000, autoPaginate: true,
            fields: ["trashed_at", "name", "path_collection", "size", "description"]);
        var changedItems = trashedFiles.Entries.Where(x => x.TrashedAt > request.Memory.LastInteractionDate).ToArray();

        if (changedItems.Length == 0)
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTimeOffset.UtcNow
                }
            };
        return new()
        {
            FlyBird = true,
            Memory = new()
            {
                LastInteractionDate = DateTimeOffset.UtcNow
            },
            Result = new()
            {
                Files = changedItems.Select(x => new PollingFileResponse(x))
            }
        };
    }

    private async Task<ICollection<BoxItem>> GetAllFiles()
    {
        var files = new List<BoxItem>();
        await FillInFiles(files);

        return files;
    }

    private async Task FillInFiles(ICollection<BoxItem> files, string folderId = "0", string folderPath = "")
    {
        var items = await _client.FoldersManager.GetFolderItemsAsync(folderId, 1000, autoPaginate: true,
            fields: ["name", "path_collection", "size", "description", "created_at", "modified_at"]);

        foreach (var item in items.Entries)
        {
            if (item.Type == "file")
                files.Add(item);

            if (item.Type == "folder")
            {
                var subfolderPath = folderPath == "" ? item.Name : folderPath + "/" + item.Name;
                await FillInFiles(files, item.Id, subfolderPath);
            }
        }

        if (items.TotalCount > files.Count)
            await FillInFiles(files, folderId, folderPath);
    }
}