using System.Threading.Tasks;
using wpay.Library.Frameworks.PayQueue;

namespace wpay.Library.Frameworks.PayQueue.Consume
{
    public interface ICallbackExecutor
    {
        Task Execute(byte[] data, Context context);
    }
}