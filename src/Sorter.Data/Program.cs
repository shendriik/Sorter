namespace Sorter.Data
{
    using Contracts;
    using Logic;
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
                        .AddTransient<IDataPreparer, TestFilePreparer>()
                        .AddTransient<IFaker, FakerWithDuplicates>();

                    services
                        .AddOptions<Settings>()
                        .Bind(context.Configuration.GetSection(nameof(Settings)))
                        .Validate(s => !string.IsNullOrWhiteSpace(s.FilePath),
                            $"{nameof(Settings.FilePath)} must be specified, use command line argument, ex.: {nameof(Settings)}:{nameof(Settings.FilePath)}=c:\\folder\\input.txt")
                        .Validate(s => s.SizeInBytes != 0,
                            $"File {nameof(Settings.SizeInBytes)} must be specified, use command line argument, ex.: {nameof(Settings)}:{nameof(Settings.SizeInBytes)}=1073741824")
                        .Validate(s => s.MaxNumber != 0,
                            $"{nameof(Settings.MaxNumber)} for generator must be specified in settings")
                        .Validate(s => s.MinWordsCount != 0,
                            $"{nameof(Settings.MinWordsCount)} for generator must be specified in settings")
                        .Validate(s => s.MaxWordsCount != 0,
                            $"{nameof(Settings.MaxWordsCount)} for generator must be specified in settings")
                        .Validate(s => s.DuplicateEachLineNumber != 0,
                            $"{nameof(Settings.DuplicateEachLineNumber)} for generator must be specified in settings");
                });
    }
}