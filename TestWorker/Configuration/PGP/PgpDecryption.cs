namespace TestWorker.Configuration
{
    public interface IPgpDecryption
    {
        string Name { get; }

        string PrivateKey { get; }

        string SecretKey { get; }
    }

    public class PgpDecryption : IPgpDecryption, IKey<string>
    {
        public string Name { get; set; }

        public string PrivateKey { get; set; }

        public string SecretKey { get; set; }

        public string Key => Name;
    }
}
