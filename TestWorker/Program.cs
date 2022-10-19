using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestWorker.Configuration;
using TestWorker.Configuration.Connection;
using TestWorker.Configuration.ContentFileTransfer;
using TestWorker.ContentFileService;
using TestWorker.Converter;
using TestWorker.Cryptography;
using TestWorker.TransferService;

namespace TestWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton(RegisterConfigurationProvider<IS3Connection, S3Connection>("Tandem:S3Connection", hostContext.Configuration));
                    services.AddSingleton(RegisterConfigurationProvider<ISftpConnection, SftpConnection>("Tandem:SftpConnection", hostContext.Configuration));
                    services.AddSingleton(RegisterConfigurationProvider<IContentFileTransfer, ContentFileTransfer>("Tandem:FileTransfer", hostContext.Configuration));
                    services.AddSingleton(RegisterConfigurationProvider<IPgpDecryption, PgpDecryption>("Tandem:Cryptography:Pgp:Decryption", hostContext.Configuration));
                    services.AddSingleton(RegisterConfigurationProvider<IPgpEncryption, PgpEncryption>("Tandem:Cryptography:Pgp:Encryption", hostContext.Configuration));
                    services.AddSingleton<IContentFileTransferServiceFactory, ContentFileTransferServiceFactory>();
                    services.AddSingleton<IContentFileServiceFactory, ContentFileServiceFactory>();
                    services.AddSingleton<ICryptographyDecryptorFactory, CryptographyDecryptorFactory>();
                    services.AddSingleton<ICryptographyEncryptorFactory, CryptographyEncryptorFactory>();
                    services.AddSingleton<IConvertFactory, ConvertFactory>();

                    services.AddHostedService<Worker>();
                });

        private static IConfigurationProvider<TConfiguration> RegisterConfigurationProvider<TConfiguration, TImplementation>(string section, IConfiguration configuration) where TImplementation: IKey<string> where TConfiguration : class
        {
            InMemoryConfigurationProviderGeneric<TConfiguration> storage = new InMemoryConfigurationProviderGeneric<TConfiguration>();

            TImplementation[] configArray = configuration.GetSection(section)
                                                         .Get<TImplementation[]>();

            foreach (TImplementation config in configArray)
            {
                storage.AddSettingsProvider(config.Key, config as TConfiguration);
            }

            return storage;
        }
    }
}
