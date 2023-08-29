using Box.V2.Models;

namespace Apps.Box.Dtos;

public class FileDto : BoxItemDto
{
    public FileDto(BoxFile file) : base(file)
    {
        File = file.Id;
    }
    
    public string File { get; set; }
}