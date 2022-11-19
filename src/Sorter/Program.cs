namespace Sorter
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Logic;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

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
                        .Configure<Settings>(context.Configuration.GetSection(nameof(Settings)))
                        .AddHostedService<WorkerService>()
                        .AddTransient<IDataStoreBuilder<string>, FileStoreBuilder>()
                        .AddTransient<ISorter<string>, Sorter>()
                        .AddTransient<IMerger<string>, KWayMerger<string>>()
                        .AddSingleton<IDataConverter<string>, RowConverter>()
                        .AddTransient<StringDefaultComparer>()
                        .AddTransient<RowComparer>()
                        .AddTransient<Func<bool, IComparer<string>>>(serviceProvider => key =>
                            key switch
                            {
                                true => serviceProvider.GetService<StringDefaultComparer>(),
                                false => serviceProvider.GetService<StringDefaultComparer>()
                            });
                });
    }
}