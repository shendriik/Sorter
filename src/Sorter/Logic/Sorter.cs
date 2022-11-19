namespace Sorter.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.Options;

    internal sealed class Sorter: ISorter<string>
    {
        private readonly Settings settings;
        private readonly IDataConverter<string> dataConverter;
        private readonly IDataStoreBuilder storeBuilder;
        private readonly Func<bool, IComparer<string>> comparerFunc;
        private readonly IMerger<string> merger;
        private readonly IComparer<string> comparer;
        
        public Sorter(
            IDataConverter<string> dataConverter,
            IOptions<Settings> settings,
            IDataStoreBuilder storeBuilder, 
            Func<bool, IComparer<string>> comparerFunc,
            IMerger<string> merger)
        {
            this.settings = settings.Value;
            this.dataConverter = dataConverter;
            this.storeBuilder = storeBuilder;
            this.comparerFunc = comparerFunc;
            this.merger = merger;
            this.comparer = comparerFunc(true);
        }
        
        public async Task SortAsync(IDataStore<string> source, IDataStore<string> output, CancellationToken cancellationToken = default)
        {
            var sortedParts = new List<IDataStore<string>>();
            var buffer = new string[settings.BufferSizeKb * 1024 / dataConverter.DataSize];

            await PartSortAsync(source, buffer, sortedParts, cancellationToken);

            if (sortedParts.Count == 0)
            {
                return;
            }

            await MergeAsync(output, sortedParts, cancellationToken);
        }

        private async Task PartSortAsync(IDataStore<string> source, string[] buffer, ICollection<IDataStore<string>> sortedParts, CancellationToken cancellationToken)
        {
            source.OpenRead();
            
            while (true)
            {
                var read = await source.GetBulkDataAsync(buffer);
                if (read == 0)
                {
                    break;
                }

                Array.Sort(buffer, 0, (int)read, comparer);

                var sortedPart = storeBuilder.Build(sortedParts.Count.ToString(), writeConvert: true);
                sortedPart.OpenWrite();
                
                for (var write = 0; write < read; write++)
                {
                    var data = buffer[write];
                    await sortedPart.WriteDataAsync(data, cancellationToken);
                }

                sortedPart.Close();
                var bufferedPartReader = storeBuilder.Build(sortedPart);
                sortedParts.Add(bufferedPartReader);
            }

            source.Close();
        }
        
        private async Task MergeAsync(IDataStore<string> output, IReadOnlyCollection<IDataStore<string>> sortedParts, CancellationToken cancellationToken)
        {
            Parallel.ForEach(sortedParts, p => p.OpenRead());
            output.OpenRead();
            
            await merger.MergeAsync(sortedParts, output, cancellationToken);
            
            Parallel.ForEach(sortedParts, p => p.Close());
            output.Close();
            
            //TODO: delete parts
        }
    }
}