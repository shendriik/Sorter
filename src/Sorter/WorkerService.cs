namespace Sorter
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Shared.Models;

    internal sealed class WorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory factory;
        private readonly IHostApplicationLifetime lifetime;
        private readonly ILogger<WorkerService> logger;
        private readonly Settings settings;

        public WorkerService(
            IServiceScopeFactory factory,
            IHostApplicationLifetime lifetime,
            ILogger<WorkerService> logger,
            IOptions<Settings> settings)
        {
            this.factory = factory;
            this.lifetime = lifetime;
            this.logger = logger;
            this.settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = factory.CreateScope();
            var sorter = scope.ServiceProvider.GetRequiredService<ISorter<DataItem>>();
            var storeBuilder = scope.ServiceProvider.GetRequiredService<IDataStoreBuilder>();
            
            using var src = storeBuilder.Build<DataItem>(settings.SourceFileName);
            using var dest = storeBuilder.Build<DataItem>(settings.DestinationFileName);
            
            try
            {
                await sorter.SortAsync(src, dest, stoppingToken);
                logger.LogInformation("Test data sort complete");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Test data sort failed");
            }
            
            lifetime.StopApplication();
        }
    }
}