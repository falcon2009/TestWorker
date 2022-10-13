using System.Threading.Tasks;

namespace TestWorker.MockService
{
    public interface IServiceBus
    {
        Task PublishAsync(object @message);
    }
}
