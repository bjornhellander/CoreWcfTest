using System.Collections.Generic;
using System.Threading.Tasks;
using CoreWCF;
using CoreWCF.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WcfTest.CoreWcf.Server
{
    internal class ServiceStarter
    {
        private readonly int port;
        private readonly List<IHostableService> services;
        private IWebHost host;

        public ServiceStarter(int port, List<IHostableService> services)
        {
            this.port = port;
            this.services = services;
        }

        public void Start()
        {
            var webHostBuilder = CreateWebHostBuilder(port, services);
            host = webHostBuilder.Build();
            host.Start();
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

        public async Task StopAsync()
        {
            await host.StopAsync();
        }
    }
}
