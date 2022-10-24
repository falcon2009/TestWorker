using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.MockService;

namespace TestWorker.Cryptography
{
    public interface ICryptographyDecryptorFactory
    {
        Task<ICryptographyDecryptor> GetCryptographyDecryptorAsync(IFeatureRelationConfiguration configuration);
    }

    public class CryptographyDecryptorFactory : ICryptographyDecryptorFactory
    {
        private readonly IConfigurationProvider<IPgpDecryption> pgpDecryptionProvider;
        private readonly IFeatureFlagService featureFlagService;

        public CryptographyDecryptorFactory(
            IConfigurationProvider<IPgpDecryption> pgpDecryptionProvider,
            IFeatureFlagService featureFlagService)
        {
            this.pgpDecryptionProvider = pgpDecryptionProvider;
            this.featureFlagService = featureFlagService;
        }
        public async Task<ICryptographyDecryptor> GetCryptographyDecryptorAsync(IFeatureRelationConfiguration configuration)
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
                "Pgp" => GetPgpCryptographyDecryptor(configuration.Name),
                _ => throw new ArgumentException("Invalid cryptography type"),
            };
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

        private ICryptographyDecryptor GetPgpCryptographyDecryptor(string name)
        {
            IPgpDecryption configuration = pgpDecryptionProvider.FirstOrDefault(config => config.Name == name);

            return new PgpDecryptor(configuration);
        }
    }
}
