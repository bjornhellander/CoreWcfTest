using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfTest.Interface
{
    [ServiceContract(CallbackContract = typeof(IChatServiceCallback))]
    public interface IChatService
    {
        [OperationContract]
        Task ConnectAsync();

        [OperationContract]
        Task PostMessageAsync(string message);
    }
}
