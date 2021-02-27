using System;

namespace WcfTest.Interface
{
    public static class ServiceInformation
    {
        public static readonly Uri BaseAddress = new Uri("net.tcp://localhost:9000/");
        public static readonly int Port = 9000;
    }
}
