namespace TestWorker.Configuration.Connection
{
    public interface IFtpConnection
    {
        public string Name { get; }
        public IFtpConfiguration FtpConfiguration { get; }
    }

    public class FtpConnection : IFtpConnection, IKey<string>
    {
        public string Name { get; set; }
        public FtpConfiguration FtpConfiguration { get; set; }
        public string Key => Name;
        IFtpConfiguration IFtpConnection.FtpConfiguration => FtpConfiguration;
    }
}
