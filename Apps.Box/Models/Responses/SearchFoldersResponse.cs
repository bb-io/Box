using Box.V2.Models;

namespace Apps.Box.Models.Responses;

public class SearchFoldersResponse
{

    public IEnumerable<BoxItem> Folders { get; set; }
}
