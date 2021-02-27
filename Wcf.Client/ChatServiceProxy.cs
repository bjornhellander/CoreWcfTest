using System.ServiceModel;
using System.Threading.Tasks;
using WcfTest.Interface;

namespace WcfTest.Wcf.Client
{
    internal class ChatServiceProxy : IChatService, IChatServiceCallback
    {
        private readonly IChatService channel;

        public ChatServiceProxy()
        {
            var binding = new NetTcpBinding();
            var addr = new EndpointAddress(ServiceInformation.BaseAddress + ChatServiceInformation.Name);
            var channelFactory = new DuplexChannelFactory<IChatService>(this, binding, addr);
            channel = channelFactory.CreateChannel();
        }

        public delegate void MessagePostedHandler(string message);

        public event MessagePostedHandler MessagePosted;

        public async Task ConnectAsync()
        {
            await channel.ConnectAsync();
        }

        public async Task PostMessageAsync(string message)
        {
            await channel.PostMessageAsync(message);
        }

        Task IChatServiceCallback.MessagePostedAsync(string message)
        {
            MessagePosted?.Invoke(message);
            return Task.CompletedTask;
        }
    }
}
