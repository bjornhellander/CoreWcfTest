using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfTest.Interface
{
    [ServiceContract]
    public interface ITimeServiceCallback
    {
        [OperationContract]
        Task TimeUpdatedAsync(DateTime now);
    }
}
