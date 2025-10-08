using Apps.Box.Dtos;

namespace Apps.Box.Models.Responses;

public class SearchFoldersResponse
{

    public IEnumerable<BasicFolderDto> Folders { get; set; }
}
