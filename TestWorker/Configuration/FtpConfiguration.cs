namespace TestWorker.Configuration
{
    public interface IFtpConfiguration
    {
        string Host { get; }
        int Port { get; }
        string UserName { get; }
        string PrivateKey { get; }
    }

    public class FtpConfiguration : IFtpConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string PrivateKey { get; set; }
    }
}
