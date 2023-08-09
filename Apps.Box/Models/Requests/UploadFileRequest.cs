namespace Apps.Box.Models.Requests
{
    public class UploadFileRequest
    {
        public byte[] File { get; set; }

        public string FileName { get; set; }

        public string ParentFolderId { get; set; }
    }
}
