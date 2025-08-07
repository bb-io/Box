using Apps.Box.Events.Polling;
using Apps.Box.Events.Polling.Models;
using Apps.Box.Events.Polling.Models.Memory;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Apps.Box.Base;

namespace Tests.Apps.Box
{
    [TestClass]
    public class PollingTests :TestBase
    {
        [TestMethod]
        public async Task OnFilesAddedOrUpdated_IsSuccess()
        {
            var polling = new PollingList(InvocationContext);
            var result = await polling.OnFilesAddedOrUpdated(new PollingEventRequest<DateMemory>
            {
                Memory = new DateMemory
                {
                    LastInteractionDate = DateTimeOffset.UtcNow.AddDays(-1)
                }
            }, new ParentFolderInput
            {
                FolderId = "334735225784"
            });
           var json = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }
    }
}
