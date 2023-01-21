using System;

namespace Interface
{
    public static class EndpointNameFactory
    {
        public static string Create(Type serviceType)
        {
            return serviceType.Name;
        }
    }
}
