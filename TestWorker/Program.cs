using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    services.AddSingleton(RegisterConfigurationProvider<IFtpConnection, FtpConnection>("Tandem:FtpConnection", hostContext.Configuration));
                    services.AddHostedService<Worker>();
                });

        //private static ITreasuryExportProviderStorage RegisterTreasuryExportProviderStorage(IConfiguration configuration)
        //{
        //    InMemoryTreasuryExportProviderStorage storage = new InMemoryTreasuryExportProviderStorage();

        //    TreasuryConfigurationExport[] providerArray = configuration.GetSection("Tandem:TreasuryConfiguration:Export")
        //                                                          .Get<TreasuryConfigurationExport[]>();

        //    return storage;
        //}

        //private static ITreasuryConfigurationImport RegisterTreasuryConfigurationImport(IConfiguration configuration)
        //{
        //    TreasuryConfigurationImport importConfiguration = new TreasuryConfigurationImport
        //    {
        //        PgpConfigurationPrivate = configuration.GetSection("Tandem:TreasuryConfiguration:Import:PGP").Get<PgpConfigurationPrivate>()
        //    };

        //    return importConfiguration;
        //}

        private static IConfigurationProvider<TConfiguration> RegisterConfigurationProvider<TConfiguration, TImplementation>(string section, IConfiguration configuration) where TImplementation: IKey<string> where TConfiguration : class
        {
            InMemoryConfigurationProviderGeneric<TConfiguration> storage = new InMemoryConfigurationProviderGeneric<TConfiguration>();

            TImplementation[] providerArray = configuration.GetSection(section)
                                                           .Get<TImplementation[]>();

            foreach (TImplementation provider in providerArray)
            {
                storage.AddSettingsProvider(provider.Key, provider as TConfiguration);
            }

            return storage;
        }
    }
}
