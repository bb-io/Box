using Apps.Box.Models.Responses;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Box.Actions;

[ActionList]
public class DebugActions(InvocationContext invocationContext) : BoxInvocable(invocationContext)
{
    [Action("Debug", Description = "Search for files in a folder")]
    public DebugResponse Debug()
    {
        return new DebugResponse
        {
            AccessToken = Creds.First(p => p.KeyName == "access_token").Value,
        };
    }
}