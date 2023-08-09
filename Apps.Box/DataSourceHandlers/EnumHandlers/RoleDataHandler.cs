using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Box.DataSourceHandlers.EnumHandlers;

public class RoleDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
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