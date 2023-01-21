using System.ServiceModel;

namespace Wcf.Client
{
    internal static class MyDuplexChannelFactory
    {
        public static TServiceContract Create<TServiceContract>(object callbackObject)
        {
            var binding = new NetTcpBinding();
            var addr = EndpointAddressFactory.Create(typeof(TServiceContract));
            var channelFactory = new DuplexChannelFactory<TServiceContract>(callbackObject, binding, addr);
            return channelFactory.CreateChannel();
        }
    }
}
