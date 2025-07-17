using Blackbird.Applications.SDK.Blueprints.Interfaces.FileStorage;
using Blackbird.Applications.Sdk.Common;
using Box.V2.Models;

namespace Apps.Box.Dtos;

public class FileDto : BoxItemDto, IDownloadFileInput
{
    public FileDto(BoxItem item, string id)  : base(item)
    { 
        FileId = id;
    }

    public FileDto(BoxFile file) : base(file)
    {
        FileId = file.Id;
    }
    
    [Display("File ID")]
    public string FileId { get; set; }
}