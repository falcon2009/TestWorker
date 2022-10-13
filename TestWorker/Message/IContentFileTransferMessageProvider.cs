using TestWorker.Model;

namespace TestWorker.Message
{
    public interface IContentFileTransferMessageProvider<TMessage> where TMessage : IContentFileTransferMessage
    {
        IContentFileTransferMessage GetContentFileTransferMessage(IContentFileTransferResult result);
    }
}
