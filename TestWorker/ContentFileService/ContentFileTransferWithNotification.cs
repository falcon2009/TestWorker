﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestWorker.Configuration;
using TestWorker.Configuration.ContentFileTransfer;
using TestWorker.Message;
using TestWorker.MockService;
using TestWorker.Model;

namespace TestWorker.ContentFileService
{
    public abstract class ContentFileTransferServiceWithNotification<TMessage> : ContentFileTransferService, IContentFileTransferService<ContentFileTransferServiceWithNotification<TMessage>>, IContentFileTransferMessageProvider<TMessage> where TMessage: IContentFileTransferMessage
    {
        private readonly IServiceBus serviceBus;

        protected ContentFileTransferServiceWithNotification(
            IContentFileTransfer contentFileTransfer,
            IContentFileServiceFactory contentFileServiceFactory,
            IPgpService pgpService,
            IConfigurationProvider<IPgpDecryption> pgpDecryptionConfigurationProvider,
            IConfigurationProvider<IPgpEncryption> pgpEncryptionConfigurationProvider,
            IServiceBus serviceBus) : base(contentFileTransfer, contentFileServiceFactory, pgpService, pgpDecryptionConfigurationProvider, pgpEncryptionConfigurationProvider)
        {
            this.serviceBus = serviceBus;
        }

        public override async Task<IEnumerable<IContentFileTransferResult>> TransferContentFileAllAsync()
        {
            IEnumerable<IContentFileTransferResult> resultArray = await base.TransferContentFileAllAsync();
            await Task.WhenAll(resultArray.Select(result => SendNotification(result)));

            return resultArray;
        }
        public override async Task<IContentFileTransferResult> TransferContentFileAsync(string name)
        {
            IContentFileTransferResult result = await base.TransferContentFileAsync(name);
            await SendNotification(result);

            return result;
        }

        public abstract IContentFileTransferMessage GetContentFileTransferMessage(IContentFileTransferResult result);

        private Task SendNotification(IContentFileTransferResult result)
        {
            if (result != null)
            {
                return Task.CompletedTask;
            }

            IContentFileTransferMessage message = GetContentFileTransferMessage(result);

            return serviceBus.PublishAsync(message);
        }
    }
}
