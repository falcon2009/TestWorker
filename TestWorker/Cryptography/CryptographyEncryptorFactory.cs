using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.MockService;

namespace TestWorker.Cryptography
{
    public interface ICryptographyEncryptorFactory
    {
        Task<ICryptographyEncryptor> GetCryptographyEncryptorAsync(IFeatureRelationConfiguration configuration);
    }

    public class CryptographyEncryptorFactory : ICryptographyEncryptorFactory
    {
        private readonly IConfigurationProvider<IPgpEncryption> pgpDecryptionProvider;
        private readonly IFeatureFlagService featureFlagService;

        public CryptographyEncryptorFactory(
            IConfigurationProvider<IPgpEncryption> pgpEncryptionProvider,
            IFeatureFlagService featureFlagService)
        {
            this.pgpDecryptionProvider = pgpEncryptionProvider;
            this.featureFlagService = featureFlagService;
        }
        public async Task<ICryptographyEncryptor> GetCryptographyEncryptorAsync(IFeatureRelationConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration?.Type) || configuration.Type == "None")
            {
                return null;
            }

            if (!await IsFeautureEnabledAsync(configuration.FeatureFlag))
            {
                return null;
            }

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

        private async Task<bool> IsFeautureEnabledAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return true;
            }

            Type featureType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(type => type.FullName.Contains(name));
            Type serviceType = featureFlagService.GetType();

            Task<bool> task = serviceType.GetMethod("IsEnabledAsync", new Type[] { typeof(int?) })
                                         .MakeGenericMethod(featureType)
                                         .Invoke(featureFlagService, new object[] { null }) as Task<bool>;

            return await task.ConfigureAwait(false);
        }
    }
}
