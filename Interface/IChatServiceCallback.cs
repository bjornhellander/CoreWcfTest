using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfTest.Interface
{
    [ServiceContract]
    public interface IChatServiceCallback
    {
        [OperationContract]
        Task MessagePostedAsync(string message);
    }
}
