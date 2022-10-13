namespace TestWorker.Configuration
{
    public interface IManagerConfiguration : IServiceConnectionConfiguration
    {
        string Encryption { get; }
    }

    public class ManagerConfiguration : ServiceConnectionConfiguration, IManagerConfiguration
    {
        public string Encryption { get; set; }
    }
}
