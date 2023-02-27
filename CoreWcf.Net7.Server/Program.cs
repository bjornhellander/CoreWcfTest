using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreWCF;
using CoreWCF.Configuration;
using Interface;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                var webHostBuilder = CreateWebHostBuilder(ServiceInformation.Port, services);
                var host = webHostBuilder.Build();
                await host.StartAsync();

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                await host.StopAsync();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(int portNumber, IReadOnlyList<IHostableService> services)
        {
            var webHostBuilder = WebHost.CreateDefaultBuilder()
                .UseKestrel(kestrelServerOptions =>
                {
                    kestrelServerOptions.ListenAnyIP(0); // 0 means any available port
                })
                .UseNetTcp(portNumber)
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddServiceModelServices();
                    foreach (var service in services)
                    {
                        serviceCollection.AddSingleton(service.GetType(), service);
                    }
                })
                .Configure(applicationBuilder =>
                 {
                     applicationBuilder.UseServiceModel(serviceBuilder =>
                     {
                         foreach (var service in services)
                         {
                             serviceBuilder.AddService(service.GetType());

                             foreach (var serviceContractType in service.ServiceContracts)
                             {
                                 var binding = new NetTcpBinding();
                                 var address = EndpointAddressFactory.Create(serviceContractType);
                                 serviceBuilder.AddServiceEndpoint(
                                     service.GetType(),
                                     serviceContractType,
                                     binding,
                                     address,
                                     listenUri: null);
                             }
                         }
                     });
                 })
                .ConfigureLogging((context, logging) =>
                {
                    logging.ClearProviders();
                });

            return webHostBuilder;
        }
    }
}
