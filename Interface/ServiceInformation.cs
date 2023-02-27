using System;

namespace WcfTest.Interface
{
    public static class ServiceInformation
    {
        public static readonly Uri ClientBaseAddress = new Uri("net.tcp://localhost:9000/");
        public static readonly Uri ServerBaseAddress = new Uri("net.tcp://0:9000/");
        public static readonly int Port = 9000;
    }
}
