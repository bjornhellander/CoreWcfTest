using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WcfTest.CoreWcf.Server;
using WcfTest.Interface;
using WcfTest.Wcf.Client;

namespace CoreWcf.Test
{
    [TestClass]
    public class ChatServiceTest
    {
        private readonly ConcurrentBag<string> postedMessages = new ConcurrentBag<string>();

        [TestMethod]
        public async Task TestMethodAsync()
        {
            var services = new List<IHostableService> { new ChatService() };
            var starter = new ServiceStarter(ServiceInformation.Port, services);
            starter.Start();

            var chatProxy = new ChatServiceProxy();
            chatProxy.MessagePosted += HandleMessagePosted;
            await chatProxy.ConnectAsync();

            await chatProxy.PostMessageAsync("hello");
            await Task.Delay(2000);
            await starter.StopAsync();

            Assert.AreEqual(1, postedMessages.Count);
            Assert.IsTrue(postedMessages.TryTake(out var postedMessage));
            Assert.AreEqual("hello", postedMessage);
        }

        private void HandleMessagePosted(string message)
        {
            postedMessages.Add(message);
        }
    }
}
