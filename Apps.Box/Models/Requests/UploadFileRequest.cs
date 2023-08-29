using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Box.Models.Requests
{
    public class UploadFileRequest
    {
        public File File { get; set; }

        [DataSource(typeof(FolderDataSourceHandler))]
        [Display("Parent folder")]
        public string ParentFolderId { get; set; }
    }
}
