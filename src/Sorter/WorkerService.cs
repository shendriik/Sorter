namespace Sorter
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal sealed class WorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory factory;
        private readonly IHostApplicationLifetime lifetime;
        private readonly ILogger<WorkerService> logger;
        private readonly IDataConverter<string> dataConverter;
        private readonly Settings settings;

        public WorkerService(
            IServiceScopeFactory factory,
            IHostApplicationLifetime lifetime,
            IDataConverter<string> dataConverter,
            ILogger<WorkerService> logger,
            IOptions<Settings> settings)
        {
            this.factory = factory;
            this.lifetime = lifetime;
            this.logger = logger;
            this.dataConverter = dataConverter;
            this.settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = factory.CreateScope();
            var sorter = scope.ServiceProvider.GetRequiredService<ISorter<string>>();
            var storeBuilder = scope.ServiceProvider.GetRequiredService<IDataStoreBuilder>();

            var watch = new Stopwatch();
            watch.Start();
            
            using var prepareRunSource = storeBuilder.Build(settings.SourceFileName);
            prepareRunSource.OpenRead();
            await dataConverter.ConfigureFromSourceAsync(prepareRunSource);
            prepareRunSource.Close();

            using var source = storeBuilder.Build(settings.SourceFileName, readConvert: true);
            using var dest = storeBuilder.Build(settings.DestinationFileName);
            
            try
            {
                await sorter.SortAsync(source, dest, stoppingToken);
                
                watch.Stop();
                logger.LogInformation($"Test data sort complete in {watch.Elapsed.TotalSeconds} sec.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Test data sort failed");
            }
            
            lifetime.StopApplication();
        }
    }
}