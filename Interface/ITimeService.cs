using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfTest.Interface
{
    [ServiceContract(CallbackContract = typeof(ITimeServiceCallback))]
    public interface ITimeService
    {
        [OperationContract]
        Task ConnectAsync();
    }
}
