using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.DataSourceHandlers.EnumHandlers;

public class RoleDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem("editor", "Editor"),
            new DataSourceItem("viewer", "Viewer"),
            new DataSourceItem("previewer", "Previewer"),
            new DataSourceItem("uploader", "Uploader"),
            new DataSourceItem("previewer uploader", "Previewer uploader"),
            new DataSourceItem("viewer uploader", "Viewer uploader"),
            new DataSourceItem("co-owner", "Co-owner"),
            new DataSourceItem("owner", "Owner")
        ];
    }
}