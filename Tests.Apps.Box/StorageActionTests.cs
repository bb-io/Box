using Apps.Box.Actions;
using Apps.Box.Models.Requests;
using Microsoft.Extensions.Configuration;
using Tests.Apps.Box.Base;

namespace Tests.Apps.Box;

[TestClass]
public class StorageActionTests : TestBase
{
    private StorageActions _actions;

    [TestInitialize]
    public void Init()
    {
        var outputDirectory = Path.Combine(GetTestFolderPath(), "Output");
        if (Directory.Exists(outputDirectory))
            Directory.Delete(outputDirectory, true);
        Directory.CreateDirectory(outputDirectory);

        _actions = new StorageActions(InvocationContext, FileManager);
    }

    [TestMethod]
    public void FileUpload_InputCorrect_Success()
    {
        var request = new UploadFileRequest()
        {
            File = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = "test.txt" }
        };

        Assert.IsNotNull(_actions.UploadFile(request));

    }

    [TestMethod]
    public async void DownloadFile_IsSuccess()
    {
        var action = new StorageActions(InvocationContext, FileManager);

        var response = await action.DownloadFile(new DownloadFileRequest()
        {
            FileId= "1910276562374"
        });

        Assert.IsNotNull(response);
    }

    private string GetTestFolderPath()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        
        return config["TestFolder"];
    }
}