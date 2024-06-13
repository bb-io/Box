using Apps.Box.Events.Polling.Models;
using Apps.Box.Events.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;

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
    public async Task<PollingEventResponse<DateMemory, ListFilesResponse>> OnFilesAddedOrUpdated(
        PollingEventRequest<DateMemory> request)
    {
        if (request.Memory == null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastIterractionDate = DateTimeOffset.UtcNow
                }
            };
        }

        var changedItems = await _client.SearchManager.QueryAsync("file", type: "file");

        if (changedItems.TotalCount == 0)
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastIterractionDate = DateTimeOffset.UtcNow
                }
            };
        return new()
        {
            FlyBird = true,
            Memory = new()
            {
                LastIterractionDate = DateTimeOffset.UtcNow
            },
            Result = new()
            {
                Files = changedItems.Entries.Select(x => new PollingFileResponse(x))
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
                    LastIterractionDate = DateTimeOffset.UtcNow
                }
            };
        }

        var trashedFiles = await _client.FoldersManager.GetTrashItemsAsync(limit: 1000, autoPaginate: true,
            fields: ["trashed_at", "name", "path_collection", "size", "description"]);
        var changedItems = trashedFiles.Entries.Where(x => x.TrashedAt > request.Memory.LastIterractionDate).ToArray();

        if (changedItems.Length == 0)
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastIterractionDate = DateTimeOffset.UtcNow
                }
            };
        return new()
        {
            FlyBird = true,
            Memory = new()
            {
                LastIterractionDate = DateTimeOffset.UtcNow
            },
            Result = new()
            {
                Files = changedItems.Select(x => new PollingFileResponse(x))
            }
        };
    }
}