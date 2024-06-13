using Blackbird.Applications.Sdk.Common.Dictionaries;

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