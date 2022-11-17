namespace Sorter.Contracts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Describes sorted data K-way merging functionality
    /// </summary>
    internal interface IMerger<TData>
    {
        /// <summary>
        /// Merge sorted data
        /// </summary>
        /// <param name="parts">Parts to merge</param>
        /// <param name="dest">Output data</param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="TData"></typeparam>
        public Task MergeAsync(
            IReadOnlyCollection<IDataStore<TData>> parts,
            IDataStore<TData> dest,
            CancellationToken cancellationToken = default);
    }
}