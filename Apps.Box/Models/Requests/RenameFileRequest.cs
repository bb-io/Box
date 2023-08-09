namespace Apps.Box.Models.Requests
{
    public class RenameFileRequest
    {
        public string FileId { get; set; }

        public string NewFilename { get; set; }
    }
}
