using Interface;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Unity;
using WcfTest.Interface;

namespace WcfTest.Wcf.Server
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("Configuring DI container...");
                var container = new UnityContainer();
                container.RegisterType<IHostableService, ChatService>(nameof(ChatService));
                container.RegisterType<IHostableService, TimeService>(nameof(TimeService));

                Console.WriteLine("Starting services...");
                var services = container.ResolveAll<IHostableService>();
                var serviceHosts = new List<ServiceHost>();
                foreach (var service in services)
                {
                    serviceHosts.Add(StartService(service));
                }

                Console.WriteLine("Service running");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

                foreach (var serviceHost in serviceHosts)
                {
                    StopService(serviceHost);
                }
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
            catch (Exception)
            {
            }
        }

        private static ServiceHost StartService(IHostableService service)
        {
            var selfHost = new ServiceHost(service, ServiceInformation.BaseAddress);

            foreach (var serviceContractType in service.ServiceContracts)
            {
                var binding = new NetTcpBinding();
                var endpointName = EndpointNameFactory.Create(serviceContractType);
                selfHost.AddServiceEndpoint(serviceContractType, binding, endpointName);
            }

            selfHost.Open();
            return selfHost;
        }

        private static void StopService(ServiceHost serviceHost)
        {
            serviceHost.Close();
        }
    }
}
