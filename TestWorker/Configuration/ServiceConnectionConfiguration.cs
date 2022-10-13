namespace TestWorker.Configuration
{
    public interface IServiceConnectionConfiguration
    {
        string ConnectionName { get; }
        string ConnectionType { get; }
        string Folder { get; }
    }

    public class ServiceConnectionConfiguration: IServiceConnectionConfiguration
    {
        public string ConnectionName { get; set; }
        public string ConnectionType { get; set; }
        public string Folder { get; set; }
    }
}
