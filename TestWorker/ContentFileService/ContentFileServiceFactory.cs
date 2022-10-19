namespace TestWorker.ContentFileService
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TestWorker.Configuration;
    using TestWorker.Configuration.Connection;
    using TestWorker.ContentFileBase;
    using TestWorker.InMemoryContentFile;
    using TestWorker.S3ContentFile;
    using TestWorker.SftpContentFile;

    public interface IContentFileServiceFactory
    {
        Task<IContentFileService> GetContentFileServiceAsync(IServiceConnectionConfiguration configuration);
    }

    public class ContentFileServiceFactory : IContentFileServiceFactory
    {
        private readonly IConfigurationProvider<ISftpConnection> ftpConnectionProvider;
        private readonly IConfigurationProvider<IS3Connection> s3ConnectionProvider;
        
        public ContentFileServiceFactory(
            IConfigurationProvider<ISftpConnection> ftpConnectionProvider,
            IConfigurationProvider<IS3Connection> s3ConnectionProvider)
        {
            this.ftpConnectionProvider = ftpConnectionProvider;
            this.s3ConnectionProvider = s3ConnectionProvider;
        }

        public async Task<IContentFileService> GetContentFileServiceAsync(IServiceConnectionConfiguration configuration)
        {
            return configuration.Type switch
            {
                "SFTP" => await GetFtpFileServiceAsync(configuration.Name),
                "S3" => await GetS3FileServiceAsync(configuration.Name),
                "Memory" => await GetInMemoryFileServiceAsync(configuration.Name),
                _ => throw new ArgumentException("Invalid connection type"),
            };
        }

        private Task<IContentFileService> GetFtpFileServiceAsync(string name)
        {
            ISftpConnection ftpConnection = ftpConnectionProvider.FirstOrDefault(connection => connection.Name == name);
            IContentFileService service = new SftpContentFileService(ftpConnection.SftpConfiguration);

            return Task.FromResult(service);
        }

        private Task<IContentFileService> GetS3FileServiceAsync(string name)
        {
            IS3Connection ftpConnection = s3ConnectionProvider.FirstOrDefault(connection => connection.Name == name);
            IContentFileService service = new S3ContentFileService(ftpConnection.S3Configuration);

            return Task.FromResult(service);
        }

        private async Task<IContentFileService> GetInMemoryFileServiceAsync(string name)
        {
            return name switch
            {
                "NatWest" => await GetInMemoryNatwestContentFileService(),
                "AccessPay" => await GetInMemoryAccessPayContentFileServiceAsync(),
                _ => throw new ArgumentException("Invalid connection type"),
            };
        }

        private async Task<IContentFileService> GetInMemoryNatwestContentFileService()
        {
            InMemoryNatwestContentFileService service = new InMemoryNatwestContentFileService();
            await service.SetupStorageAsync();

            return service;
        }

        private async Task<IContentFileService> GetInMemoryAccessPayContentFileServiceAsync()
        {
            InMemoryAccessPayContentFileService service = new InMemoryAccessPayContentFileService();
            await service.SetupStorageAsync();

            return service;
        }
    }
}
