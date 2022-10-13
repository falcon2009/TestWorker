using System;
using System.Linq;
using TestWorker.Configuration;
using TestWorker.Configuration.Connection;
using TestWorker.ContentFileBase;
using TestWorker.S3ContentFile;
using TestWorker.SftpContentFile;

namespace TestWorker.ContentFileService
{
    public interface IContentFileProviderFactory
    {
        IContentFileProvider GetContentFileProvider(IServiceConnectionConfiguration configuration);
    }

    public class ContentFileProviderFactory : IContentFileProviderFactory
    {
        private readonly IConfigurationProvider<ISftpConnection> ftpConnectionProvider;
        private readonly IConfigurationProvider<IS3Connection> s3ConnectionProvider;
        
        public ContentFileProviderFactory(
            IConfigurationProvider<ISftpConnection> ftpConnectionProvider,
            IConfigurationProvider<IS3Connection> s3ConnectionProvider)
        {
            this.ftpConnectionProvider = ftpConnectionProvider;
            this.s3ConnectionProvider = s3ConnectionProvider;
        }

        public IContentFileProvider GetContentFileProvider(IServiceConnectionConfiguration configuration)
        {
            return configuration.ConnectionType switch
            {
                "SFTP" => GetFtpFileProvider(configuration.ConnectionName),
                "S3" => GetS3FileProvider(configuration.ConnectionName),
                _ => throw new ArgumentException("Invalid connection type"),
            };
        }

        private IContentFileProvider GetFtpFileProvider(string name)
        {
            ISftpConnection ftpConnection = ftpConnectionProvider.FirstOrDefault(connection => connection.Name == name);

            return new SftpContentFileService(ftpConnection.SftpConfiguration);
        }

        private IContentFileProvider GetS3FileProvider(string name)
        {
            IS3Connection ftpConnection = s3ConnectionProvider.FirstOrDefault(connection => connection.Name == name);

            return new S3ContentFileService(ftpConnection.S3Configuration);
        }
    }
}
