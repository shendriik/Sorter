namespace Sorter
{
    using System;
    using System.Collections.Generic;
    using Contracts;
    using Logic;
    using Logic.Files;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public sealed class Program
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
                        .AddTransient<ISorter<string>, Sorter>()
                        .AddTransient<IMerger<string>, KWayMerger<string>>()
                        .AddSingleton<IDataConverter<string>, RowConverter>()
                        .AddTransient<IComparer<string>, StringDefaultComparer>();

                    services
                        .AddOptions<Settings>()
                        .Bind(context.Configuration.GetSection(nameof(Settings)))
                        .Validate(s => !string.IsNullOrWhiteSpace(s.Path),
                            $"Files {nameof(Settings.Path)} must be specified, use command line argument, ex.: {nameof(Settings)}:{nameof(Settings.Path)}=c:\\folder")
                        .Validate(s => !string.IsNullOrWhiteSpace(s.DestinationFileName),
                            $"{nameof(Settings.DestinationFileName)} must be specified, use command line argument, ex.: {nameof(Settings)}:{nameof(Settings.DestinationFileName)}=output.txt")
                        .Validate(s => !string.IsNullOrWhiteSpace(s.SourceFileName),
                            $"{nameof(Settings.SourceFileName)} must be specified, use command line argument, ex.: {nameof(Settings)}:{nameof(Settings.SourceFileName)}=input.txt")
                        .Validate(s => s.StringBufferLength != 0,
                            $"{nameof(Settings.StringBufferLength)} for string RAM buffer must be specified in settings");
                });
    }
}
