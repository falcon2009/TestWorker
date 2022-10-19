namespace TestWorker.Configuration
{
    public interface IProviderConfiguration : IServiceConnectionConfiguration
    {
        bool IsOneTypeFileOnly { get; }
    }

    public class ProviderConfiguration : ServiceConnectionConfiguration, IProviderConfiguration
    {
        public bool IsOneTypeFileOnly { get; set; }
    }
}
