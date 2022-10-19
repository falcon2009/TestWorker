using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.Configuration.ContentFileTransfer;
using TestWorker.ContentFileBase;
using TestWorker.ContentFileService;
using TestWorker.Converter;
using TestWorker.Cryptography;

namespace TestWorker.TransferService
{
    public interface IContentFileTransferServiceFactory
    {
        Task<IContentFileTransferService> GetContentFileTransferServiceAsync(string fileType, string sourse, string destination);
    }

    public class ContentFileTransferServiceFactory : IContentFileTransferServiceFactory
    {
        private readonly IConfigurationProvider<IContentFileTransfer> configurationProvider;
        private readonly IContentFileServiceFactory contentFileServiceFactory;
        private readonly ICryptographyEncryptorFactory cryptographyEncryptorFactory;
        private readonly ICryptographyDecryptorFactory cryptographyDecryptorFactory;
        private readonly IConvertFactory convertFactory;

        public ContentFileTransferServiceFactory(
            IConfigurationProvider<IContentFileTransfer> configurationProvider,
            IContentFileServiceFactory contentFileServiceFactory,
            ICryptographyDecryptorFactory cryptographyDecryptorFactory,
            ICryptographyEncryptorFactory cryptographyEncryptorFactory,
            IConvertFactory convertFactory)
        {
            this.configurationProvider = configurationProvider;
            this.contentFileServiceFactory = contentFileServiceFactory;
            this.cryptographyDecryptorFactory = cryptographyDecryptorFactory;
            this.cryptographyEncryptorFactory = cryptographyEncryptorFactory;
            this.convertFactory = convertFactory;
        }

        public async Task<IContentFileTransferService> GetContentFileTransferServiceAsync(string fileType, string sourse, string destination)
        {
            IContentFileTransfer configuration = configurationProvider.FirstOrDefault(config => config.FileType == fileType && config.Source == sourse && config.Destination == destination);
            if (configuration?.Provider == null || configuration?.Manager == null)
            {
                throw new Exception("Invalid file transfer configuration");
            }

            IContentFileService sourceContentFileService = await contentFileServiceFactory.GetContentFileServiceAsync(configuration.Provider);
            IContentFileService destinationContentFileService = await contentFileServiceFactory.GetContentFileServiceAsync(configuration.Provider);
            ICryptographyDecryptor cryptographyDecryptor = cryptographyDecryptorFactory.GetCryptographyDecryptor(configuration.Pipeline?.Decrypt);
            ICryptographyEncryptor cryptographyEncryptor = cryptographyEncryptorFactory.GetCryptographyEncryptor(configuration.Pipeline?.Encrypt);
            IConverter converter = convertFactory.GetConverter(configuration.Pipeline?.Convert);

            return new ContentFileTransferService(
                configuration,
                sourceContentFileService,
                destinationContentFileService,
                cryptographyDecryptor,
                cryptographyEncryptor,
                converter);
        }
    }
}
