using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Apps.Box.DataSourceHandlers.EnumHandlers;

public class StatusDataHander : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "accepted", "Accepted" },
        { "rejected", "Rejected" }
    };
}