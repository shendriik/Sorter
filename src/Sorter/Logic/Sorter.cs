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
        private readonly IDataStoreBuilder storeBuilder;
        private readonly IMerger<string> merger;
        private readonly IComparer<string> comparer;
        
        public Sorter(
            IOptions<Settings> settings,
            IDataStoreBuilder storeBuilder, 
            IComparer<string> comparer,
            IMerger<string> merger)
        {
            this.settings = settings.Value;
            this.storeBuilder = storeBuilder;
            this.comparer = comparer;
            this.merger = merger;
            this.comparer = comparer;
        }
        
        public async Task SortAsync(IDataStore<string> source, IDataStore<string> output, CancellationToken cancellationToken = default)
        {
            var buffer = new string[settings.StringBufferLength];
            
            var sortedParts = new List<IDataStore<string>>();
            await PartSortAsync(source, buffer, sortedParts, cancellationToken);

            if (sortedParts.Count == 0)
            {
                return;
            }
            
            var partsToMerge = CreatePartsToMerge(buffer, sortedParts);

            await MergeAsync(output, partsToMerge, cancellationToken);
        }

        private List<IDataStore<string>> CreatePartsToMerge(string[] buffer, List<IDataStore<string>> sortedParts)
        {
            var partBufferSize = (int)Math.Ceiling((float)buffer.Length / sortedParts.Count);
            var partsToMerge = new List<IDataStore<string>>(sortedParts.Count);
            for (var index = 0; index < sortedParts.Count; index++)
            {
                var sortedPart = sortedParts[index];

                var size = Math.Min(partBufferSize, buffer.Length - index * partBufferSize);
                var partToMerge = storeBuilder.Build(sortedPart, buffer, index * partBufferSize, size);
                partsToMerge.Add(partToMerge);
            }

            return partsToMerge;
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
                
                //TODO: multiple strings alloc
                GC.Collect();
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
            
            //TODO: delete useless parts
        }
    }
}