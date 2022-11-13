namespace Sorter.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Describes sorted data merging functionality
    /// </summary>
    internal interface IMerger
    {
        /// <summary>
        /// Merge sorted data
        /// </summary>
        /// <param name="src1">Source data</param>
        /// <param name="src2">Source data</param>
        /// <param name="dest">Output data</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TData"></typeparam>
        public Task MergeAsync<TData>(
            IDataStore<TData> src1,
            IDataStore<TData> src2,
            IDataStore<TData> dest,
            CancellationToken cancellationToken = default)
            where TData : IComparable<TData>;
    }
}