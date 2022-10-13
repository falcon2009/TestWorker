using System;
using System.Linq;
using TestWorker.Configuration;
using TestWorker.Configuration.Connection;
using TestWorker.ContentFileBase;
using TestWorker.S3ContentFile;
using TestWorker.SftpContentFile;

namespace TestWorker.ContentFileService
{
    public interface IContentFileManagerFactory
    {
        IContentFileManager GetContentFileManager(IServiceConnectionConfiguration configuration);
    }

    public class ContentFileManagerFactory : IContentFileManagerFactory
    {
        private readonly IConfigurationProvider<ISftpConnection> ftpConnectionProvider;
        private readonly IConfigurationProvider<IS3Connection> s3ConnectionProvider;
        
        public ContentFileManagerFactory(
            IConfigurationProvider<ISftpConnection> ftpConnectionProvider,
            IConfigurationProvider<IS3Connection> s3ConnectionProvider)
        {
            this.ftpConnectionProvider = ftpConnectionProvider;
            this.s3ConnectionProvider = s3ConnectionProvider;
        }

        public IContentFileManager GetContentFileManager(IServiceConnectionConfiguration configuration)
        {
            return configuration.ConnectionType switch
            {
                "SFTP" => GetFtpFileManager(configuration.ConnectionName),
                "S3" => GetS3FileManager(configuration.ConnectionName),
                _ => throw new ArgumentException("Invalid connection type"),
            };
        }

        private IContentFileManager GetFtpFileManager(string name)
        {
            ISftpConnection ftpConnection = ftpConnectionProvider.FirstOrDefault(connection => connection.Name == name);

            return new SftpContentFileService(ftpConnection.SftpConfiguration);
        }

        private IContentFileManager GetS3FileManager(string name)
        {
            IS3Connection ftpConnection = s3ConnectionProvider.FirstOrDefault(connection => connection.Name == name);

            return new S3ContentFileService(ftpConnection.S3Configuration);
        }
    }
}
