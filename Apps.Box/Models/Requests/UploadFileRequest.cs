using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Box.Models.Requests
{
    public class UploadFileRequest
    {
        public File File { get; set; }

        public string ParentFolderId { get; set; }
    }
}
