namespace Sorter.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides data sorting using external data stores abstraction
    /// </summary>
    internal interface ISorter<TData>
    {
        /// <summary>
        /// Sorts data
        /// </summary>
        /// <param name="source">Input data abstraction</param>
        /// <param name="output">Output data abstraction</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SortAsync(IDataStore<TData> source, IDataStore<TData> output, CancellationToken cancellationToken = default);
    }
}