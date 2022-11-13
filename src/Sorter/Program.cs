namespace Sorter
{
    using Contracts;
    using Logic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Shared.Models;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddHostedService<WorkerService>()
                        .AddTransient<IDataStoreBuilder, FileStoreBuilder>()
                        .AddTransient<IDataItemFactory, DataItemFactory>()
                        .AddTransient<IMerger, BufferedMerger>()
                        .AddTransient<ISorter<DataItem>, Sorter<DataItem>>()
                        .Configure<Settings>(context.Configuration.GetSection(nameof(Settings)));
                });
    }
}