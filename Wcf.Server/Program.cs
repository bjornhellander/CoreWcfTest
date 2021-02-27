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
                var chatServiceHost = StartService(typeof(ChatService), typeof(IChatService), ChatServiceInformation.Name);
                var timeServiceHost = StartService(typeof(TimeService), typeof(ITimeService), TimeServiceInformation.Name);

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

        private static ServiceHost StartService(Type serviceType, Type serviceContractType, string endpointName)
        {
            var selfHost = new ServiceHost(serviceType, ServiceInformation.BaseAddress);
            var binding = new NetTcpBinding();
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
