namespace Sorter.Logic.Files
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class BufferedFileStore : IDataStore<string>
    {
        private readonly IDataStore<string> store;

        public BufferedFileStore(IDataStore<string> store)
        {
            this.store = store;
        }

        public void OpenRead()
        {
            store.OpenRead();
        }

        public void OpenWrite()
        {
            store.OpenWrite();
        }

        public Task<long> GetBulkDataAsync(string[] buffer)
        {
            return store.GetBulkDataAsync(buffer);
        }

        public Task<string> GetDataAsync()
        {
            return store.GetDataAsync();
        }

        public Task WriteDataAsync(string data, CancellationToken cancellationToken)
        {
            return store.WriteDataAsync(data, cancellationToken);
        }

        public bool IsEnd()
        {
            return store.IsEnd();
        }

        public long Length()
        {
            return store.Length();
        }

        public void Close()
        {
            store.Close();
        }
        
        public void Dispose()
        {
            store.Dispose();
        }
    }
}