using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;

namespace Apps.Box.Models.Requests;

public class WebhookFolderRequest
{
    [Display("Folder", Description = "Folder to monitor, including its subfolders")]
    [FileDataSource(typeof(FolderPickerDataSourceHandler))]
    public string FolderId { get; set; } = string.Empty;
}
