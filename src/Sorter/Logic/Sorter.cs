namespace Sorter.Logic
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;
    using Contracts;
    using Microsoft.Extensions.Logging;
    using Shared.Models;

    internal sealed class Sorter<TData>: ISorter<TData> where TData : BaseDataItem, IComparable<TData>
    {
        private readonly IMerger merger;
        private readonly IDataStoreBuilder storeBuilder;
        private readonly ILogger<Sorter<TData>> logger;

        public Sorter(IMerger merger, IDataStoreBuilder storeBuilder, ILogger<Sorter<TData>> logger)
        {
            this.merger = merger;
            this.storeBuilder = storeBuilder;
            this.logger = logger;
        }
        
        private const int ChunkSize = 3*1024*1024;
        private TData[] buffer;
        private Queue<IDataStore<TData>> stores;
        
        public async Task SortAsync(IDataStore<TData> source, IDataStore<TData> output, CancellationToken cancellationToken = default)
        {
            buffer = new TData[ChunkSize];
            stores = new Queue<IDataStore<TData>>();
            
            var watch = new Stopwatch();
            
            
            source.OpenRead();
            while (!source.IsEnd())
            {
                watch.Restart();
                var read = await source.GetBulkDataAsync(buffer);
                if (read == 0)
                {
                    break;
                }
                watch.Stop();
                logger.LogInformation($"Read part {watch.Elapsed.TotalSeconds} sec.");
                
                watch.Restart();
                Array.Sort(buffer, 0, (int)read);
                watch.Stop();
                logger.LogInformation($"Sort part {watch.Elapsed.TotalSeconds} sec.");
                
                watch.Restart();
                var sorted = storeBuilder.Build<TData>(stores.Count.ToString());
                sorted.OpenWrite();
                
                // write by lines
                for (var index2 = 0; index2 < read; index2++)
                {
                    var data = buffer[index2];
                    await sorted.WriteDataAsync(data, cancellationToken);
                }
                watch.Stop();
                logger.LogInformation($"Write part {watch.Elapsed.TotalSeconds} sec.");

                sorted.Close();
                stores.Enqueue(sorted);
            }

            source.Close();
            
            watch.Stop();
            logger.LogInformation($"Split data into {stores.Count} sorted parts in {watch.Elapsed.TotalSeconds} sec.");
            
            if (stores.Count == 0)
            {
                return;
            }
            
            watch.Restart();
            var cycles = 0;
            do
            {
                cycles++;

                stores.TryDequeue(out var src1);
                stores.TryDequeue(out var src2);

                var merged = stores.Count == 0 ? output : storeBuilder.Build<TData>(CreateStoreName(src1?.Name, src2?.Name));

                src1?.OpenRead();
                src2?.OpenRead();
                merged.OpenWrite();
                
                await merger.MergeAsync(src1, src2, merged, cancellationToken);
                stores.Enqueue(merged);

                logger.LogInformation($"Merged part {src1?.Name}, {src2?.Name} into part {merged.Name}");
            } 
            while (stores.Count > 1);
            output.Close();
            
            watch.Stop();
            logger.LogInformation($"Merges complete in ({cycles} cycles) in {watch.Elapsed.TotalSeconds} sec.");
        }

        private string CreateStoreName(string store1, string store2)
            => Guid.NewGuid().ToString(); // (store1 ?? "_") + (store2 ?? "_");
    }
}