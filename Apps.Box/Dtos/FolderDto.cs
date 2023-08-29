using Box.V2.Models;

namespace Apps.Box.Dtos;

public class FolderDto : BoxItemDto
{
    public FolderDto(BoxFolder folder) : base(folder)
    {
        Folder = folder.Id;
    }
    
    public string Folder { get; set; }
}