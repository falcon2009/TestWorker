namespace TestWorker.Configuration
{
    public interface IProviderConfiguration : IServiceConnectionConfiguration
    {
        string Decryption { get; }
    }

    public class ProviderConfiguration : ServiceConnectionConfiguration, IProviderConfiguration
    {
        public string Decryption { get; set; }
    }
}
