using TestWorker.Configuration.Service;

namespace TestWorker.Configuration
{
    public interface IFileTransferConfiguration
    {
        string FileType { get; }
        string Master { get; }
        IServiceConnection Provider { get; }
        IServiceConnection Manager { get; }
    }
}
