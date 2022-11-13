namespace Sorter.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IDataStore<TData> : IDisposable
    {
        string Name { get; }
        
        void OpenRead();
        
        void OpenWrite();
        
        void Close();

        Task<long> GetBulkDataAsync(TData[] buffer);
        
        Task<TData> GetDataAsync();

        Task WriteBulkDataAsync(TData[] data, int start, long length, CancellationToken cancellationToken = default);
        
        Task WriteDataAsync(TData data, CancellationToken cancellationToken = default);

        bool IsEnd();
        
        long Length();
    }
}