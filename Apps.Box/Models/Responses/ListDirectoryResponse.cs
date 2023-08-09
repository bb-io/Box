using Apps.Box.Dtos;

namespace Apps.Box.Models.Responses
{
    public class ListDirectoryResponse
    {
        public IEnumerable<DirectoryItemDto> DirectoriesItems { get; set; }
    }
}
