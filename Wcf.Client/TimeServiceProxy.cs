using System;
using System.ServiceModel;
using System.Threading.Tasks;
using WcfTest.Interface;

namespace WcfTest.Wcf.Client
{
    internal class TimeServiceProxy : ITimeService, ITimeServiceCallback
    {
        private readonly ITimeService channel;

        public TimeServiceProxy()
        {
            var binding = new NetTcpBinding();
            var addr = new EndpointAddress(ServiceInformation.BaseAddress + TimeServiceInformation.Name);
            var channelFactory = new DuplexChannelFactory<ITimeService>(this, binding, addr);
            channel = channelFactory.CreateChannel();
        }

        public delegate void TimeUpdatedHandler(DateTime time);

        public event TimeUpdatedHandler TimeUpdated;

        public async Task ConnectAsync()
        {
            await channel.ConnectAsync();
        }

        Task ITimeServiceCallback.TimeUpdatedAsync(DateTime now)
        {
            TimeUpdated?.Invoke(now);
            return Task.CompletedTask;
        }
    }
}
