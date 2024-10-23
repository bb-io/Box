using Apps.Box.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Box.Models.Responses;

public class ListDirectoryResponse
{
    [Display("Files")]
    public IEnumerable<FileDto> Files { get; set; }
}