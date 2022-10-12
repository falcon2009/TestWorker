namespace TestWorker
{
    public interface ITreasuryConfigurationExport
    {
        public string Name { get; }
        ITreasuryS3Configuration S3Configuration { get; }
        string EncryptionType { get; }
        string EncryptionKey { get; }
    }

    public class TreasuryConfigurationExport : ITreasuryConfigurationExport
    {
        public string Name { get; set; }
        public TreasuryS3Configuration S3Configuration { get; set; }
        public string EncryptionType { get; set; }
        public string EncryptionKey { get; set; }

        ITreasuryS3Configuration ITreasuryConfigurationExport.S3Configuration => S3Configuration;
    }
}
