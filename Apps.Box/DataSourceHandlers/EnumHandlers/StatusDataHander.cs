using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Box.DataSourceHandlers.EnumHandlers;

public class StatusDataHander : IStaticDataSourceHandler
{
    public Dictionary<string, string> GetData()
        => new()
        {
            { "accepted", "Accepted" },
            { "rejected", "Rejected" }
        };
}