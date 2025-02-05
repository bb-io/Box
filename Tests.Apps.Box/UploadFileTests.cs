using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Box;
using Apps.Box.Models.Requests;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Tests.Apps.Box.Base;

namespace Tests.Apps.Box
{
    [TestClass]
    public class UploadFileTests : TestBase
    {
        private Actions _actions;

        [TestInitialize]
        public void Init()
        {
            var outputDirectory = Path.Combine(GetTestFolderPath(), "Output");
            if (Directory.Exists(outputDirectory))
                Directory.Delete(outputDirectory, true);
            Directory.CreateDirectory(outputDirectory);

            _actions = new Actions(InvocationContext, FileManager);
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

        //[TestMethod]
        //public async Task FileUpload_InputNameMissing_ThrowsError()
        //{
        //    var request = new UploadFileRequest()
        //    {
        //        File = new Blackbird.Applications.Sdk.Common.Files.FileReference() { Name = null }
        //    };

        //    await Assert.ThrowsExceptionAsync<PluginApplicationException>(() => _actions.UploadFile(request));

        //}
        private string GetTestFolderPath()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            return config["TestFolder"];
        }
    }
}
