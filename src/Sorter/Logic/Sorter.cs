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
        private readonly IDataStoreBuilder<string> storeBuilder;
        private readonly Func<bool, IComparer<string>> comparerFunc;
        private readonly IMerger<string> merger;
        private readonly IComparer<string> comparer;
        
        public Sorter(
            IDataConverter<string> dataConverter,
            IOptions<Settings> settings,
            IDataStoreBuilder<string> storeBuilder, 
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
            var sortedParts = new Queue<IDataStore<string>>();
            var buffer = new string[settings.BufferSizeKb * 1024 / dataConverter.DataSize];

            await PartSortAsync(source, buffer, sortedParts, cancellationToken);

            if (sortedParts.Count == 0)
            {
                return;
            }

            await MergeAsync(output, sortedParts, cancellationToken);
        }

        private async Task PartSortAsync(IDataStore<string> source, string[] buffer, Queue<IDataStore<string>> sortedParts, CancellationToken cancellationToken)
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

                var sortedPart = storeBuilder.Build(sortedParts.Count.ToString());
                sortedPart.OpenWrite();
                
                for (var write = 0; write < read; write++)
                {
                    var data = buffer[write];
                    await sortedPart.WriteDataAsync(data, cancellationToken);
                }

                sortedPart.Close();
                sortedParts.Enqueue(sortedPart);
            }

            source.Close();
        }
        
        private async Task MergeAsync(IDataStore<string> output, Queue<IDataStore<string>> sortedParts, CancellationToken cancellationToken)
        {
            var partNumber = sortedParts.Count;
            
            while (sortedParts.Count > 0)
            {
                var partsToMerge = new List<IDataStore<string>>();
                var deep = sortedParts.Count / settings.MergeDeep < 2 ? sortedParts.Count : settings.MergeDeep;
                while (partsToMerge.Count < deep)
                {
                    var part = sortedParts.Dequeue();
                    partsToMerge.Add(part);
                    part.OpenRead();
                }

                var merged = sortedParts.Count == 0 ? output : storeBuilder.Build(partNumber.ToString());
                merged.OpenWrite();

                await merger.MergeAsync(partsToMerge, merged, cancellationToken);

                if (sortedParts.Count > 0)
                {
                    sortedParts.Enqueue(merged);
                }
                
                Parallel.ForEach(partsToMerge, p => p.Close());
                merged.Close();
                partNumber++;
            }
            
            //TODO: delete parts
        }
    }
}