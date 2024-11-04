using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Box.Models.Requests
{
    public class CopyFileRequest
    {
        [Display("New name")]
        public string? NewName { get; set; }

        [DataSource(typeof(FileDataSourceHandler))]
        [Display("File ID")]
        public string FileId { get; set; }


        [DataSource(typeof(FolderDataSourceHandler))]
        [Display("New parent folder ID")]
        public string ParentFolderId { get; set; }
    }
}
