namespace Apps.Box.Models.Requests
{
    public class CreateFolderRequest
    {
        public string FolderName { get; set; }

        public string ParentFolderId { get; set; }
    }
}
