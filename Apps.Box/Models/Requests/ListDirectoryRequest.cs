namespace Apps.Box.Models.Requests
{
    public class ListDirectoryRequest
    {
        public string FolderId { get; set; }

        public int Limit { get; set; }
    }
}
