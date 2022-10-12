namespace TestWorker
{
    public interface IPgpConfigurationPrivate
    {
        string PrivateKey { get; }
        string KeySecret { get; }
    }

    public class PgpConfigurationPrivate : IPgpConfigurationPrivate
    {
        public string PrivateKey { get; set; }
        public string KeySecret { get; set; }
    }
}
