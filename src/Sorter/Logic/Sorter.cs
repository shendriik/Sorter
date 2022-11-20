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
        private readonly IMerger<string> merger;
        private readonly IComparer<string> comparer;
        
        public Sorter(
            IDataConverter<string> dataConverter,
            IOptions<Settings> settings,
            IDataStoreBuilder storeBuilder, 
            IComparer<string> comparer,
            IMerger<string> merger)
        {
            this.settings = settings.Value;
            this.dataConverter = dataConverter;
            this.storeBuilder = storeBuilder;
            this.comparer = comparer;
            this.merger = merger;
            this.comparer = comparer;
        }
        
        public async Task SortAsync(IDataStore<string> source, IDataStore<string> output, CancellationToken cancellationToken = default)
        {
            var bufferSize = settings.BufferSizeKb * 1024 / dataConverter.DataSize ;
            var buffer = new string[bufferSize];
            
            var sortedParts = new List<IDataStore<string>>();
            await PartSortAsync(source, buffer, sortedParts, cancellationToken);

            if (sortedParts.Count == 0)
            {
                return;
            }
            
            var partBufferSize = (int)Math.Ceiling((float)bufferSize / sortedParts.Count);
            var partsToMerge = new List<IDataStore<string>>(sortedParts.Count);
            for (var index = 0; index < sortedParts.Count; index++)
            {
                var sortedPart = sortedParts[index];

                var size = Math.Min(partBufferSize, bufferSize - index * partBufferSize);
                var partToMerge = storeBuilder.Build(sortedPart, buffer, index * partBufferSize, size);
                partsToMerge.Add(partToMerge);
            }

            await MergeAsync(output, partsToMerge, cancellationToken);
        }

        private async Task PartSortAsync(IDataStore<string> source, string[] buffer, ICollection<IDataStore<string>> sortedParts, CancellationToken cancellationToken)
        {
            source.OpenRead();
            
            while (true)
            {
                var read = await source.GetBulkDataAsync(buffer, 0, buffer.Length);
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
                sortedParts.Add(sortedPart);
                
                //GC.Collect();
            }

            source.Close();
        }
        
        private async Task MergeAsync(IDataStore<string> output, IReadOnlyCollection<IDataStore<string>> sortedParts, CancellationToken cancellationToken)
        {
            Parallel.ForEach(sortedParts, p => p.OpenRead());
            output.OpenWrite();
            
            await merger.MergeAsync(sortedParts, output, cancellationToken);
            
            Parallel.ForEach(sortedParts, p => p.Close());
            output.Close();
            
            //TODO: delete parts
        }
    }
}