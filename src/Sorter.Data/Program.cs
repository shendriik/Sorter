namespace Sorter.Data
{
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
                        .AddHostedService<WorkerService>()
                        .AddTransient<IDataPreparer, TestFilePreparer>()
                        .AddTransient<IFaker, FakerWithDuplicates>()
                        .Configure<Settings>(context.Configuration.GetSection(nameof(Settings)));
                });
    }
}