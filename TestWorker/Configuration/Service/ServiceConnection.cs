namespace TestWorker.Configuration.Service
{
    public interface IServiceConnection
    {
        string ConnectionName { get; }
        string ConnectionType { get; }
        string Folder { get; }
    }

    public class ServiceConnection: IServiceConnection
    {
        public string ConnectionName { get; set; }
        public string ConnectionType { get; set; }
        public string Folder { get; set; }
    }
}
