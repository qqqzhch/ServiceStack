#if !SILVERLIGHT && !MONOTOUCH && !XBOX && !ANDROIDINDIE
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ServiceStack
{
    [ServiceContract(Namespace = "http://services.servicestack.net/", CallbackContract = typeof(IDuplexCallback))]
    public interface IDuplex
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        void BeginSend(Message msg);
    }
}
#endif