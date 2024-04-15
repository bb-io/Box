using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Box.DataSourceHandlers.EnumHandlers;

public class RoleDataHandler : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
        => new()
        {
            { "editor", "Editor" },
            { "viewer", "Viewer" },
            { "previewer", "Previewer" },
            { "uploader", "Uploader" },
            { "previewer uploader", "Previewer uploader" },
            { "viewer uploader", "Viewer uploader" },
            { "co-owner", "Co-owner" },
            { "owner", "Owner" }
        };
}