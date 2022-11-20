namespace Sorter.Logic.Files
{
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class BufferedFileStore : IDataStore<string>
    {
        private readonly IDataStore<string> store;
        private readonly string[] buffer;
        private readonly int startIndex;
        
        private readonly long bufferSize;
        private long bufferedSize;
        private long readPosition;
        private bool sourceIsEnd;
        
        public BufferedFileStore(IDataStore<string> store, string[] buffer, int startIndex, long bufferSize)
        {
            this.store = store;
            this.buffer = buffer;
            this.startIndex = startIndex;
            this.bufferSize = bufferSize;
        }

        public void OpenRead()
        {
            store.OpenRead();

            bufferedSize = 0;
            readPosition = 0;
        }

        public void OpenWrite()
        {
            store.OpenWrite();
        }

        public Task<long> GetBulkDataAsync(string[] buff, int bulkStartIndex, long bulkSize)
        {
            return store.GetBulkDataAsync(buff, bulkStartIndex, bulkSize);
        }
        
        public async Task<string> GetDataAsync()
        {
            if (sourceIsEnd)
            {
                return null;
            }
            
            if (bufferedSize - readPosition == 0)
            {
                readPosition = 0; 
                bufferedSize = await store.GetBulkDataAsync(buffer, startIndex, bufferSize);

                if (bufferedSize == 0)
                {
                    sourceIsEnd = true;
                    return null;
                }
            }

            return buffer[startIndex + readPosition++];
        }

        public Task WriteDataAsync(string data, CancellationToken cancellationToken)
        {
            return store.WriteDataAsync(data, cancellationToken);
        }

        public bool IsEnd()
        {
            return sourceIsEnd;
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