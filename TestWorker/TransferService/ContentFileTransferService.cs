using Microsoft.Extensions.Logging;
using Nito;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using TestWorker.Configuration.ContentFileTransfer;
using TestWorker.ContentFileBase;
using TestWorker.Converter;
using TestWorker.Cryptography;
using TestWorker.Model;

namespace TestWorker.TransferService
{
    public interface IContentFileTransferService
    {
        Task<IEnumerable<IContentFileTransferResult>> TransferContentFileAllAsync();
        Task<IContentFileTransferResult> TransferContentFileAsync(string name);
    }

    public sealed class ContentFileTransferService : IContentFileTransferService, IDisposable
    {
        private readonly IList<IDisposable> disposableList = new List<IDisposable>();
        private readonly IContentFileTransfer configuration;
        private readonly IContentFileService sourceContentFileService;
        private readonly IContentFileService destinationContentFileService;
        private readonly ICryptographyDecryptor cryptographyDecryptor;
        private readonly ICryptographyEncryptor cryptographyEncryptor;
        private readonly IConverter converter;


        public ContentFileTransferService(
            IContentFileTransfer configuration,
            IContentFileService sourceContentFileService,
            IContentFileService destinationContentFileService,
            ICryptographyDecryptor cryptographyDecryptor,
            ICryptographyEncryptor cryptographyEncryptor,
            IConverter converter)
        {
            this.configuration = configuration;
            this.sourceContentFileService = sourceContentFileService;
            this.destinationContentFileService = destinationContentFileService;
            this.cryptographyDecryptor = cryptographyDecryptor;
            this.cryptographyEncryptor = cryptographyEncryptor;
            this.converter = converter;
        }

        public async Task<IEnumerable<IContentFileTransferResult>> TransferContentFileAllAsync()
        {
            IPropagatorBlock<string, IContentFileTransferResult> pipeLine = CreateTransferPipelineForContentFile();
            IEnumerable<IContentFile> contentFileArray = await sourceContentFileService.GetListAsync(configuration.Provider.Folder);

            Func<IContentFile, bool> predicate = !configuration.Provider.IsOneTypeFileOnly ? (item) => !string.IsNullOrEmpty(item?.Name)
                                                                                           : (item) => !string.IsNullOrEmpty(item?.Name) && item.Name.Contains(configuration.FileType);
            contentFileArray = contentFileArray.Where(predicate);
            IEnumerable<IContentFileTransferResult> result = 
                await Task.WhenAll(contentFileArray.Select(contentFile => contentFile.Name)
                                                   .Select(async name => {
                                                       await pipeLine.SendAsync(name);
                                                       return await pipeLine.ReceiveAsync();
                                                   }));

            return result.Where(item => item != null);
        }

        public async Task<IContentFileTransferResult> TransferContentFileAsync(string name)
        {
            IPropagatorBlock<string, IContentFileTransferResult> pipeLine = CreateTransferPipelineForContentFile();
            await pipeLine.SendAsync(name);

            return await pipeLine.ReceiveAsync();

        }

        private IPropagatorBlock<string, IContentFileTransferResult> CreateTransferPipelineForContentFile()
        {
            DataflowLinkOptions options = new DataflowLinkOptions { PropagateCompletion = true };
            TransformBlock<string, (string, byte[])> sourceBlock = CreateFetchBlock();
            TransformBlock<(string, byte[]), (string, byte[])> decryptorBlock = CreateDecriptorBlock();
            TransformBlock<(string, byte[]), (string, byte[])> convertBlock = CreateConverterBlock();
            TransformBlock<(string, byte[]), (string, byte[])> encryptBlock = CreateEncryptBlock();
            TransformBlock<(string, byte[]), IContentFileTransferResult> resultBlock = CreateUploadAndDeleteBlock();
            disposableList.Add(sourceBlock.LinkTo(decryptorBlock, options));
            disposableList.Add(decryptorBlock.LinkTo(convertBlock, options));
            disposableList.Add(convertBlock.LinkTo(encryptBlock, options));
            disposableList.Add(encryptBlock.LinkTo(resultBlock, options));

            return  DataflowBlock.Encapsulate(sourceBlock, resultBlock);
        }

        private TransformBlock<string, (string, byte[])> CreateFetchBlock()
        {
            TransformBlock<string, (string, byte[])> block = new TransformBlock<string, (string, byte[])>( async name => {
                using IContentFile source = await sourceContentFileService.GetAsync(GetKey(name));

                return (name, source.Stream.ToArray());
            });


            return block;
        }

        private TransformBlock<(string, byte[]), (string, byte[])> CreateDecriptorBlock()
        {
            TransformBlock<(string, byte[]), (string, byte[])> block = new TransformBlock<(string, byte[]), (string, byte[])>(async entry => {
                if (cryptographyDecryptor == null)
                {
                    return entry;
                }

                return (entry.Item1, await cryptographyDecryptor.DecryptAsync(entry.Item2));
            });

            return block;
        }

        private TransformBlock<(string, byte[]), (string, byte[])> CreateConverterBlock()
        {
            TransformBlock<(string, byte[]), (string, byte[])> block = new TransformBlock<(string, byte[]), (string, byte[])>(entry => {
                if (converter == null)
                {
                    return entry;
                }

                return (entry.Item1, converter.Convert(entry.Item2));
            });

            return block;
        }

        private TransformBlock<(string, byte[]), (string, byte[])> CreateEncryptBlock()
        {
            TransformBlock<(string, byte[]), (string, byte[])> block = new TransformBlock<(string, byte[]), (string, byte[])>(async entry => {
                if (cryptographyEncryptor == null)
                {
                    return entry;
                }

                return (entry.Item1, await cryptographyEncryptor.EncryptAsync(entry.Item2));
            });

            return block;
        }

        private TransformBlock<(string, byte[]), IContentFileTransferResult> CreateUploadAndDeleteBlock()
        {
            TransformBlock<(string, byte[]), IContentFileTransferResult> block = new TransformBlock<(string, byte[]), IContentFileTransferResult>(async entry => {
                using IContentFile contentFile = new ContentFile
                {
                    ContentType = "application/octem-stream",
                    Name = entry.Item1,
                    Path = configuration.Manager.Folder,
                    Stream = new MemoryStream(entry.Item2)
                };
                await destinationContentFileService.UploadAsync(contentFile);
                if (configuration.ShouldRemoveAfterUploading)
                {
                    await sourceContentFileService.DeleteAsync(GetKey(entry.Item1));
                }

                return new ContentFileTransferResult
                {
                    Source = configuration.Source,
                    Destination = configuration.Destination,
                    FileName = entry.Item1,
                    FileType = configuration.FileType
                };
            });

            return block;
        }

        private string GetKey(string name)
        {
            return string.IsNullOrEmpty(configuration.Provider.Folder) ? name
                                                                       : $"{configuration.Provider.Folder}/{name}";
        }

        private static TransformBlock<Try<TInput>, Try<TOutput>> RailwayTransform<TInput, TOutput>(Func<TInput, TOutput> func)
        {
            return new TransformBlock<Try<TInput>, Try<TOutput>>(result => result.Map(func));
        }


        public void Dispose()
        {
            foreach (IDisposable item in disposableList)
            {
                item?.Dispose();
            }
        }
    }
}
