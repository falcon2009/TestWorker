using System;
using System.Linq;
using TestWorker.Configuration;
using TestWorker.Configuration.Connection;
using TestWorker.ContentFileBase;
using TestWorker.S3ContentFile;
using TestWorker.SftpContentFile;

namespace TestWorker.ContentFileService
{
    public interface IContentFileServiceFactory
    {
        IContentFileService GetContentFileService(IServiceConnectionConfiguration configuration);
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

        public IContentFileService GetContentFileService(IServiceConnectionConfiguration configuration)
        {
            return configuration.ConnectionType switch
            {
                "SFTP" => GetFtpFileService(configuration.ConnectionName),
                "S3" => GetS3FileService(configuration.ConnectionName),
                _ => throw new ArgumentException("Invalid connection type"),
            };
        }

        private IContentFileService GetFtpFileService(string name)
        {
            ISftpConnection ftpConnection = ftpConnectionProvider.FirstOrDefault(connection => connection.Name == name);

            return new SftpContentFileService(ftpConnection.SftpConfiguration);
        }

        private IContentFileService GetS3FileService(string name)
        {
            IS3Connection ftpConnection = s3ConnectionProvider.FirstOrDefault(connection => connection.Name == name);

            return new S3ContentFileService(ftpConnection.S3Configuration);
        }
    }
}
