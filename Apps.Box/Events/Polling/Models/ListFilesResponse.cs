namespace Apps.Box.Events.Polling.Models;

public class ListFilesResponse
{
    public IEnumerable<PollingFileResponse> Files { get; set; }
}