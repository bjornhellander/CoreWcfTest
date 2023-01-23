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
                    kestrelServerOptions.ListenLocalhost(8080); // TODO: How is this relevant?
                })
                .UseNetTcp(portNumber)
                .Configure(applicationBuilder =>
                 {
                     applicationBuilder.UseServiceModel(serviceBuilder =>
                     {
                         foreach (var service in services)
                         {
                             serviceBuilder.AddService(service.GetType());

                             foreach (var serviceContractType in service.ServiceContracts)
                             {
                                 var endpointName = EndpointNameFactory.Create(serviceContractType);
                                 var binding = new NetTcpBinding();
                                 var address = new Uri(endpointName, UriKind.Relative);
                                 serviceBuilder.AddServiceEndpoint(service.GetType(), serviceContractType, binding, address, null);
                             }
                         }
                     });
                 });

            webHostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddServiceModelServices();
                foreach (var service in services)
                {
                    serviceCollection.AddSingleton(service.GetType(), sp => service);
                }
            });

            return webHostBuilder;
        }
    }
}
