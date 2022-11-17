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

        public void OpenRead()
        {
            readPosition = 0;
        }
        
        public void OpenWrite()
        {
            writePosition = 0;
        }

        public Task<long> GetBulkDataAsync(T[] output)
        {
            long index = 0;
            for (index = 0; index < output.Length && !IsEnd(); index++)
            {
                output[index] = buffer[readPosition++];
            }

            return Task.FromResult(index);
        }

        public Task<T> GetDataAsync()
        {
            return Task.FromResult(
                readPosition == writePosition
                    ? default
                    : buffer[readPosition++]);
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

        public void Close()
        {
        }
        
        public void Dispose()
        {
        }
    }
}