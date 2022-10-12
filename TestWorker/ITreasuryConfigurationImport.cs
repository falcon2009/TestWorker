
namespace TestWorker
{
    public interface ITreasuryConfigurationImport
    {
        IPgpConfigurationPrivate PgpConfigurationPrivate { get; }
    }

    public class TreasuryConfigurationImport : ITreasuryConfigurationImport
    {
        public IPgpConfigurationPrivate PgpConfigurationPrivate { get; set; }

    }
}
