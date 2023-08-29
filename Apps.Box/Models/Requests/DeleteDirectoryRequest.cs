using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests
{
    public class DeleteDirectoryRequest
    {
        [DataSource(typeof(FolderDataSourceHandler))]
        [Display("Folder")]
        public string FolderId { get; set; }
    }
}
