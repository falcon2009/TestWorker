using System;
using System.Linq;
using TestWorker.Configuration;

namespace TestWorker.Cryptography
{
    public interface ICryptographyEncryptorFactory
    {
        ICryptographyEncryptor GetCryptographyEncryptor(IRelationConfiguration configuration);
    }

    public class CryptographyEncryptorFactory : ICryptographyEncryptorFactory
    {
        private readonly IConfigurationProvider<IPgpEncryption> pgpDecryptionProvider;

        public CryptographyEncryptorFactory(IConfigurationProvider<IPgpEncryption> pgpEncryptionProvider)
        {
            this.pgpDecryptionProvider = pgpEncryptionProvider;
        }
        public ICryptographyEncryptor GetCryptographyEncryptor(IRelationConfiguration configuration)
        {
            return configuration.Type switch
            {
                "Pgp" => GetPgpCryptographyEncryptor(configuration.Name),
                _ => throw new ArgumentException("Invalid cryptography type"),
            };
        }

        private ICryptographyEncryptor GetPgpCryptographyEncryptor(string name)
        {
            IPgpEncryption configuration = pgpDecryptionProvider.FirstOrDefault(config => config.Name == name);

            return new PgpEncryptor(configuration);
        }
    }
}
