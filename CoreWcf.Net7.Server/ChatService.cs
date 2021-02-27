using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreWCF;
using WcfTest.Interface;

namespace WcfTest.CoreWcf.Server
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
    internal class ChatService : IChatService
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly List<IChatServiceCallback> callbacks = new List<IChatServiceCallback>();

        public async Task ConnectAsync()
        {
            await semaphore.WaitAsync();

            var callback = OperationContext.Current.GetCallbackChannel<IChatServiceCallback>();
            callbacks.Add(callback);

            semaphore.Release();
        }

        public async Task PostMessageAsync(string message)
        {
            await semaphore.WaitAsync();

            foreach (var callback in callbacks.ToList())
            {
                try
                {
                    await callback.MessagePostedAsync(message);
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
