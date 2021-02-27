using System;
using System.Collections.Generic;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using WcfTest.Interface;

namespace WcfTest.CoreWcf.Server
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                var services = new List<(Type ServiceType, Type ServiceContractType, string EndpointName)>
                {
                    (typeof(ChatService), typeof(IChatService), ChatServiceInformation.Name),
                    (typeof(TimeService), typeof(ITimeService), TimeServiceInformation.Name),
                };

                Console.WriteLine("Starting service...");
                var webHostBuilder = CreateWebHostBuilder(ServiceInformation.Port, services);
                var host = webHostBuilder.Build();
                host.Start();
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(int portNumber, List<(Type ServiceType, Type ServiceContractType, string EndpointName)> services)
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
                         foreach (var (serviceType, serviceContractType, endpointName) in services)
                         {
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
