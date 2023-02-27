using Interface;
using System;
using System.ServiceModel;
using WcfTest.Interface;

namespace Wcf.Client
{
    internal static class EndpointAddressFactory
    {
        public static EndpointAddress Create(Type serviceType)
        {
            return new EndpointAddress(ServiceInformation.ClientBaseAddress + EndpointNameFactory.Create(serviceType));
        }
    }
}
