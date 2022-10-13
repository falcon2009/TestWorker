using TestWorker.Model;

namespace TestWorker.Message
{
    public interface IContentFileTransferMessageProvider<out TMessage> where TMessage : IContentFileTransferMessage
    {
        TMessage CreateContentFileTransferMessage(IContentFileTransferResult result);
    }
}
