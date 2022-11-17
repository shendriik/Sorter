namespace Sorter.Data.Logic
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal sealed class TestFilePreparer : IDataPreparer
    {
        private readonly IFaker faker;
        private readonly ILogger<TestFilePreparer> logger;
        private readonly Settings settings;
            
        public TestFilePreparer(IFaker faker, IOptions<Settings> settings, ILogger<TestFilePreparer> logger)
        {
            this.faker = faker;
            this.logger = logger;
            this.settings = settings.Value;
        }
        
        public async Task ExecuteAsync(CancellationToken cancellation)
        {
            logger.LogInformation($"Starting test file '{settings.Path}' creation");

            await using var writer = new StreamWriter(settings.Path, new FileStreamOptions
            {
                Access = FileAccess.Write, 
                Mode = FileMode.Create
            });

            var watch = new Stopwatch();
            watch.Start();

            long rowsCount = 0;
            
            while (writer.BaseStream.Length < settings.SizeInBytes)
            {
                await writer.WriteLineAsync(faker.Generate().AsMemory(), cancellation);
                rowsCount++;
            }
            
            watch.Stop();
            logger.LogInformation($"File '{settings.Path}' has been created with size {writer.BaseStream.Length} bytes "+
                                  $"({rowsCount} rows) in {watch.Elapsed.TotalSeconds} sec.");
        }
    }
}