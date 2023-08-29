using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Box.Models.Requests
{
    public class GetFileInformationRequest
    {
        [DataSource(typeof(FileDataSourceHandler))]
        [Display("File")]
        public string FileId { get; set; }
    }
}
