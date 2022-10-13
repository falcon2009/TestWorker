namespace TestWorker.Configuration
{
    public interface IPgpEncryption
    {
        string Name { get; }

        string PublicKey { get; }
    }

    public class PgpEncryption : IPgpEncryption, IKey<string>
    {
        public string Name { get; set; }

        public string PublicKey { get; set; }

        public string Key => Name;
    }
}
