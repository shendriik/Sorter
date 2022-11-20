namespace Sorter.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    /// <summary>
    /// K-way merge with min heap
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    internal sealed class KWayMerger<TData> : IMerger<TData>
    {
        private readonly IComparer<TData> dataComparer;

        public KWayMerger(Func<bool, IComparer<TData>> comparerFunc)
        {
            dataComparer = comparerFunc(false);
        }
        
        public async Task MergeAsync(
            IReadOnlyCollection<IDataStore<TData>> sources,
            IDataStore<TData> dest, 
            CancellationToken cancellationToken = default)
        {
            var minHeap = new MinHeap<HeapElement>(sources.Count, new HeapElementComparer(dataComparer));
            
            foreach (var source in sources)
            {
                minHeap.Add(new HeapElement
                {
                    Data = await source.GetDataAsync(),
                    Store = source
                });
            }

            while (minHeap.Count > 0)
            {
                var element = minHeap.RemoveMin();
                await dest.WriteDataAsync(element.Data, cancellationToken);

                var data =  await element.Store.GetDataAsync();
                if (data == null)
                {
                    continue;
                }

                element.Data = data;
                minHeap.Add(element);
            }
        }

        private sealed class HeapElement
        {
            public TData Data { get; set; }
            
            public IDataStore<TData>  Store { get; set; }
        }
        
        private sealed class HeapElementComparer : Comparer<HeapElement>
        {
            private readonly IComparer<TData> dataComparer;

            public HeapElementComparer(IComparer<TData> dataComparer)
            {
                this.dataComparer = dataComparer;
            }
            public override int Compare(HeapElement x, HeapElement y)
            {
                return dataComparer.Compare(x.Data, y.Data);
            }
        }
    }
}