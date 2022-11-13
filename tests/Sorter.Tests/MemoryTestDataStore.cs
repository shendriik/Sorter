namespace Sorter.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class MemoryTestDataStore<T> : IDataStore<T>
    {
        private List<T> buffer;
        private int readPosition;
        private int writePosition;

        public MemoryTestDataStore()
        {
            buffer = new List<T>();
        }

        public string Name => "test";

        public void OpenRead()
        {
            readPosition = 0;
        }
        
        public void OpenWrite()
        {
            writePosition = 0;
        }

        public void Close()
        {
        }

        public Task<T> GetDataAsync()
        {
            return Task.FromResult(
                readPosition == writePosition
                    ? default
                    : buffer[readPosition++]);
        }

        public Task WriteBulkDataAsync(T[] data, int start, long length, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task WriteDataAsync(T data, CancellationToken cancellationToken = default)
        {
            buffer.Add(data);
            writePosition++;
            
            return Task.CompletedTask;
        }

        public bool IsEnd()
        {
            return readPosition == writePosition;
        }

        public long Length()
        {
            return writePosition;
        }

        public void Dispose()
        {
        }
    }
}