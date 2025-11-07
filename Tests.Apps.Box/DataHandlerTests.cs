using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems;
using Tests.Apps.Box.Base;

namespace Tests.Apps.Box
{
    [TestClass]
    public class DataHandlerTests : TestBase
    {
        [TestMethod]
        public async Task FileDataSourceHandler_IsSuccess()
        {
            var handler = new FileDataSourceHandler(InvocationContext);
            var files = await handler.GetDataAsync(new DataSourceContext
            {
                SearchString = "UAE Translations.xlsx"
            }, CancellationToken.None);

            foreach (var file in files)
            {
                Console.WriteLine($"{file.Value} - {file.DisplayName}");
            }
            Assert.IsNotNull(files);
        }

        [TestMethod]
        public async Task FilePickerDataSourceHandler_GetFolderContentAsync_ShouldReturnItems()
        {
            var handler = new FilePickerDataSourceHandler(InvocationContext);
            var result = await handler.GetFolderContentAsync(new Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.FolderContentDataSourceContext
            {
                FolderId = string.Empty
            }, CancellationToken.None);
            var itemList = result.ToList();
            foreach (var item in itemList)
            {
                Console.WriteLine($"Item: {item.DisplayName}, Id: {item.Id}, Type: {(item is Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.Folder ? "Folder" : "File")}");
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(itemList.Count > 0, "The folder should contain items.");
        }

        [TestMethod]
        public async Task FolderPickerDataSourceHandler_GetFolderContentAsync_ShouldReturnItems()
        {
            var handler = new FolderPickerDataSourceHandler(InvocationContext);
            var result = await handler.GetFolderContentAsync(new FolderContentDataSourceContext
            {
                FolderId = string.Empty
            }, CancellationToken.None);
            var itemList = result.ToList();
            foreach (var item in itemList)
            {
                Console.WriteLine($"Item: {item.DisplayName}, Id: {item.Id}, Type: {(item is Blackbird.Applications.SDK.Extensions.FileManagement.Models.FileDataSourceItems.Folder ? "Folder" : "File")}");
            }
            Assert.IsNotNull(result);
            Assert.IsTrue(itemList.Count > 0, "The folder should contain items.");
        }
    }
}
