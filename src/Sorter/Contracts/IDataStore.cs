namespace Sorter.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IDataStore<TData> : IDisposable
    {
        void OpenRead();
        
        void OpenWrite();
        
        Task<long> GetBulkDataAsync(TData[] buffer);

        Task<TData> GetDataAsync();

        Task WriteDataAsync(TData data, CancellationToken cancellationToken = default);

        bool IsEnd();
        
        long Length();
        
        void Close();
    }
}