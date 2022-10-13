using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.Configuration.ContentFileTransfer;
using TestWorker.ContentFileBase;
using TestWorker.MockService;
using TestWorker.Model;

namespace TestWorker.ContentFileService
{
    public interface IContentFileTransferService<T>
    {
        Task<IEnumerable<IContentFileTransferResult>> TransferContentFileAllAsync();
        Task<IContentFileTransferResult> TransferContentFileAsync(string name);
    }

    public class ContentFileTransferService : IContentFileTransferService<ContentFileTransferService>
    {
        private readonly IContentFileTransfer contentFileTransfer;
        private readonly IPgpService pgpService;
        private readonly IContentFileService sourceContentFileService;
        private readonly IContentFileService destinationContentFileService;
        private readonly IPgpDecryption pgpDecription;
        private readonly IPgpEncryption pgpEncryption;

        public ContentFileTransferService(
            IContentFileTransfer contentFileTransfer,
            IContentFileServiceFactory contentFileServiceFactory,
            IPgpService pgpService,
            IConfigurationProvider<IPgpDecryption> pgpDecryptionConfigurationProvider,
            IConfigurationProvider<IPgpEncryption> pgpEncryptionConfigurationProvider)
        {
            this.contentFileTransfer = contentFileTransfer;
            this.pgpService = pgpService;
            sourceContentFileService = contentFileServiceFactory.GetContentFileService(contentFileTransfer.Manager);
            destinationContentFileService = contentFileServiceFactory.GetContentFileService(contentFileTransfer.Provider);
            pgpDecription = pgpDecryptionConfigurationProvider.FirstOrDefault(config => config.Name == contentFileTransfer.Provider.Decryption);
            pgpEncryption = pgpEncryptionConfigurationProvider.FirstOrDefault(config => config.Name == contentFileTransfer.Manager.Encryption);
        }

        public virtual async Task<IEnumerable<IContentFileTransferResult>> TransferContentFileAllAsync()
        {
            IEnumerable<IContentFile> contentFileArray = await sourceContentFileService.GetListAsync(contentFileTransfer.Provider.Folder);

            IEnumerable<IContentFileTransferResult> result = 
                await Task.WhenAll(contentFileArray.Select(contentFile => contentFile.Name)
                                                   .Select(name => TransferContentFileAsync(name)));

            return result.Where(item => item != null);
        }

        public virtual async Task<IContentFileTransferResult> TransferContentFileAsync(string name)
        {
            try
            {
                string key = $"{contentFileTransfer.Provider.Folder}/{name}";
                using IContentFile source = await sourceContentFileService.GetAsync(key);
                using IContentFile destination = new ContentFile
                {
                    ContentType = source.ContentType,
                    Name = source.Name,
                    Path = contentFileTransfer.Manager.Folder,
                    Stream = new MemoryStream()
                };
                source.Stream.Position = 0;
                bool requiredPgpService = !(pgpDecription == null && pgpEncryption == null);
                if (!requiredPgpService)
                {
                    await source.Stream.CopyToAsync(destination.Stream);
                }
                else 
                {
                    using Stream destinationStream = GetEncryptedAndDecryptedStream(source.Stream);
                    await destinationStream.CopyToAsync(destination.Stream);
                }
                await destinationContentFileService.UploadAsync(source);
                if (contentFileTransfer.ShouldRemoveAfterUploading)
                {
                    await sourceContentFileService.DeleteAsync(key);
                }

                return new ContentFileTransferResult
                {
                    Source = contentFileTransfer.Source,
                    Destination = contentFileTransfer.Destination,
                    FileName = source.Name,
                    FileType = contentFileTransfer.FileType
                };
            }
            catch
            {
                return null;
            }
        }

        private Stream GetEncryptedAndDecryptedStream(Stream source)
        {
            byte[] decryptedData = pgpDecription == null ? null 
                                                         : pgpService.Decrypt(source, Encoding.UTF8.GetBytes(pgpDecription.PrivateKey), pgpDecription.SecretKey);

            byte[] resultData = pgpEncryption == null ? decryptedData
                                                      : decryptedData == null ? pgpService.Encrypt(source, Encoding.UTF8.GetBytes(pgpEncryption.PublicKey))
                                                                              : pgpService.Encrypt(decryptedData, Encoding.UTF8.GetBytes(pgpEncryption.PublicKey));

            return new MemoryStream(resultData);
        }
    }
}
