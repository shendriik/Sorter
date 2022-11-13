namespace Sorter.Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal sealed class WorkerService : BackgroundService
    {
        private readonly IServiceScopeFactory factory;
        private readonly IHostApplicationLifetime lifetime;
        private readonly ILogger<WorkerService> logger;

        public WorkerService(IServiceScopeFactory factory, IHostApplicationLifetime lifetime, ILogger<WorkerService> logger)
        {
            this.factory = factory;
            this.lifetime = lifetime;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = factory.CreateScope();
            var dataPreparer = scope.ServiceProvider.GetRequiredService<IDataPreparer>();

            try
            {
                await dataPreparer.ExecuteAsync(stoppingToken);
                logger.LogInformation("Test data preparing complete");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Test data preparing failed");
            }
            
            lifetime.StopApplication();
        }
    }
}