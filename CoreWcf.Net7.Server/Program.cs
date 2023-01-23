using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreWCF;
using CoreWCF.Configuration;
using Interface;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WcfTest.Interface;

namespace WcfTest.CoreWcf.Server
{
    public class Program
    {
        public static async Task Main()
        {
            try
            {
                var services = new List<(Type ServiceType, Type ServiceContractType)>
                {
                    (typeof(ChatService), typeof(IChatService)),
                    (typeof(TimeService), typeof(ITimeService)),
                };

                Console.WriteLine("Starting services...");
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

        public static IWebHostBuilder CreateWebHostBuilder(int portNumber, List<(Type ServiceType, Type ServiceContractType)> services)
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
                         foreach (var (serviceType, serviceContractType) in services)
                         {
                             var endpointName = EndpointNameFactory.Create(serviceContractType);
                             serviceBuilder
                                 .AddService(serviceType)
                                 .AddServiceEndpoint(serviceType, serviceContractType, new NetTcpBinding(), new Uri(endpointName, UriKind.Relative), null);
                         }
                     });
                 });

            webHostBuilder.ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddServiceModelServices();
            });

            return webHostBuilder;
        }
    }
}
