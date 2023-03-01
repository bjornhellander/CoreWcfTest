using System;
using System.Linq;
using System.Threading.Tasks;
using CoreWCF;
using Unity;
using WcfTest.Interface;

namespace WcfTest.CoreWcf.Server
{
    public class Program
    {
        public static async Task Main()
        {
            try
            {
                Console.WriteLine("Configuring DI container...");
                var container = new UnityContainer();
                container.RegisterType<IHostableService, ChatService>(nameof(ChatService));
                container.RegisterType<IHostableService, TimeService>(nameof(TimeService));

                Console.WriteLine("Starting services...");
                var services = container.ResolveAll<IHostableService>().ToList();
                var serviceStarter = new ServiceStarter(ServiceInformation.Port, services);
                serviceStarter.Start();

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                await serviceStarter.StopAsync();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
        }
    }
}
