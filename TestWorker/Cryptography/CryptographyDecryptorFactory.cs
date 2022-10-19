using System;
using System.Linq;
using TestWorker.Configuration;

namespace TestWorker.Cryptography
{
    public interface ICryptographyDecryptorFactory
    {
        ICryptographyDecryptor GetCryptographyDecryptor(IRelationConfiguration configuration);
    }

    public class CryptographyDecryptorFactory : ICryptographyDecryptorFactory
    {
        private readonly IConfigurationProvider<IPgpDecryption> pgpDecryptionProvider;

        public CryptographyDecryptorFactory(IConfigurationProvider<IPgpDecryption> pgpDecryptionProvider)
        {
            this.pgpDecryptionProvider = pgpDecryptionProvider;
        }
        public ICryptographyDecryptor GetCryptographyDecryptor(IRelationConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration?.Type) || configuration.Type == "None")
            {
                return null;
            }

            return configuration.Type switch
            {
                "Pgp" => GetPgpCryptographyDecryptor(configuration.Name),
                _ => throw new ArgumentException("Invalid cryptography type"),
            };
        }

        private ICryptographyDecryptor GetPgpCryptographyDecryptor(string name)
        {
            IPgpDecryption configuration = pgpDecryptionProvider.FirstOrDefault(config => config.Name == name);

            return new PgpDecryptor(configuration);
        }
    }
}
