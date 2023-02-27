using Interface;
using System;
using WcfTest.Interface;

namespace WcfTest.CoreWcf.Server
{
    internal static class EndpointAddressFactory
    {
        public static Uri Create(Type serviceType)
        {
            return new Uri(ServiceInformation.ServerBaseAddress + EndpointNameFactory.Create(serviceType));
        }
    }
}
