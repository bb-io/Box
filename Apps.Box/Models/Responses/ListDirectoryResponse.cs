using Apps.Box.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Box.Models.Responses;

public class ListDirectoryResponse
{
    [Display("Directory items")]
    public IEnumerable<DirectoryItemDto> DirectoriesItems { get; set; }
}