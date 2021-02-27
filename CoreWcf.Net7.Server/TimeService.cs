using CoreWCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WcfTest.Interface;

namespace WcfTest.CoreWcf.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    internal class TimeService : ITimeService
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly List<ITimeServiceCallback> callbacks = new List<ITimeServiceCallback>();

        public TimeService()
        {
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) => HandleTimerElapsed();
            timer.Start();
        }

        public async Task ConnectAsync()
        {
            await semaphore.WaitAsync();

            var callback = OperationContext.Current.GetCallbackChannel<ITimeServiceCallback>();
            callbacks.Add(callback);

            semaphore.Release();
        }

        private async void HandleTimerElapsed()
        {
            await semaphore.WaitAsync();

            foreach (var callback in callbacks.ToList())
            {
                try
                {
                    await callback.TimeUpdatedAsync(DateTime.UtcNow);
                }
                catch (CommunicationException)
                {
                    callbacks.Remove(callback);
                }
            }

            semaphore.Release();
        }
    }
}
