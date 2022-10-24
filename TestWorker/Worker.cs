namespace TestWorker
{
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TestWorker.Model;
    using TestWorker.TransferService;

    public class Worker : BackgroundService
    {
        private readonly IContentFileTransferServiceFactory contentFileTransferServiceFactory;
        private readonly ILogger<Worker> _logger;

        public Worker(
            IContentFileTransferServiceFactory contentFileTransferServiceFactory,
            ILogger<Worker> logger)
        {
            this.contentFileTransferServiceFactory = contentFileTransferServiceFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            IContentFileTransferService contentFileTransferService = await contentFileTransferServiceFactory.GetContentFileTransferServiceAsync("MT103", "NatWestMemory", "AccessPayMemory");
            IContentFileTransferResult[] result = (await contentFileTransferService.TransferContentFileAllAsync()).ToArray();
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
