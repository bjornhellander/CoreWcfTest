using Interface;
using System;
using System.ServiceModel;
using WcfTest.Interface;

namespace WcfTest.Wcf.Server
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Console.WriteLine("Starting service...");
                var chatServiceHost = StartService(typeof(ChatService), typeof(IChatService));
                var timeServiceHost = StartService(typeof(TimeService), typeof(ITimeService));

                Console.WriteLine("Service running");
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();

                StopService(chatServiceHost);
                StopService(timeServiceHost);
            }
            catch (CommunicationException e)
            {
                Console.WriteLine($"Caught exception: {e}");
            }
            catch (Exception)
            {
            }
        }

        private static ServiceHost StartService(Type serviceType, Type serviceContractType)
        {
            var selfHost = new ServiceHost(serviceType, ServiceInformation.BaseAddress);
            var binding = new NetTcpBinding();
            var endpointName = EndpointNameFactory.Create(serviceContractType);
            selfHost.AddServiceEndpoint(serviceContractType, binding, endpointName);
            selfHost.Open();

            return selfHost;
        }

        private static void StopService(ServiceHost serviceHost)
        {
            serviceHost.Close();
        }
    }
}
