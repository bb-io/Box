using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.DataSourceHandlers.EnumHandlers;

public class StatusDataHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return
        [
            new DataSourceItem("accepted", "Accepted"),
            new DataSourceItem("rejected", "Rejected")
        ];
    }
}