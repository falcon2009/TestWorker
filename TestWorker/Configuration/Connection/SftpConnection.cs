namespace TestWorker.Configuration.Connection
{
    public interface ISftpConnection
    {
        public string Name { get; }
        public ISftpConfiguration SftpConfiguration { get; }
    }

    public class SftpConnection : ISftpConnection, IKey<string>
    {
        public string Name { get; set; }
        public SftpConfiguration SftpConfiguration { get; set; }
        public string Key => Name;
        ISftpConfiguration ISftpConnection.SftpConfiguration => SftpConfiguration;
    }
}
