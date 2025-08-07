using Apps.Box.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Apps.Box.Base;

namespace Tests.Apps.Box
{
    [TestClass]
    public class DataHandlerTests :TestBase
    {
        [TestMethod]
        public async Task FileDataSourceHandler_IsSuccess()
        {
            var handler = new FileDataSourceHandler(InvocationContext);
            var files = await handler.GetDataAsync(new DataSourceContext
            {
                SearchString = ""
            }, CancellationToken.None);

            foreach (var file in files)
            {
                Console.WriteLine($"{file.Value} - {file.DisplayName}");
            }
            Assert.IsNotNull(files);
        }
       
    }
}
