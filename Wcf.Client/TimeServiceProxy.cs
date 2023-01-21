using System;
using System.Threading.Tasks;
using Wcf.Client;
using WcfTest.Interface;

namespace WcfTest.Wcf.Client
{
    internal class TimeServiceProxy : ITimeService, ITimeServiceCallback
    {
        private readonly ITimeService channel;

        public TimeServiceProxy()
        {
            channel = MyDuplexChannelFactory.Create<ITimeService>(this);
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
