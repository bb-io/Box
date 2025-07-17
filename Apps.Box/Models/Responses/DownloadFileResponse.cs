using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Box.Models.Responses;

public class DownloadFileResponse : IDownloadFileOutput
{
    public FileReference File { get; set; }
}