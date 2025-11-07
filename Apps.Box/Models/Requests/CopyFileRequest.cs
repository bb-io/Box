using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests
{
    public class CopyFileRequest
    {
        [Display("New name")]
        public string? NewName { get; set; }

        [FileDataSource(typeof(FilePickerDataSourceHandler))]
        [Display("File ID")]
        public string FileId { get; set; }


        [FileDataSource(typeof(FolderPickerDataSourceHandler))]
        [Display("New parent folder ID")]
        public string ParentFolderId { get; set; }

        [Display("Remove original?", Description = "If set to true, the original file will be deleted.")]
        public bool? RemoveOriginal { get; set; }
    }
}
