namespace Sorter.Logic
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;
    using Shared.Models;

    internal sealed class FileStore<TData>: IDataStore<TData> where TData: BaseDataItem
    {
        private const string FileNameFormat = "merge{0}.txt";
        private const int BufferSize = 8 * 1024;
        
        private readonly string fullPath;
        private readonly IDataItemFactory itemFactory;

        private StreamReader reader;
        private StreamWriter writer;
        
        public FileStore(string path, string name, IDataItemFactory itemFactory)
        {
            var fileName = string.IsNullOrEmpty(Path.GetExtension(name))
                ? string.Format(FileNameFormat, name)
                : name;
            fullPath = Path.Combine(path, fileName);
            
            this.itemFactory = itemFactory;
            Name = name;
        }
        
        public string Name { get; }
        
        public void OpenRead()
        { 
            reader = new StreamReader(fullPath,
                new FileStreamOptions { Access = FileAccess.Read, Mode = FileMode.Open, BufferSize = BufferSize });
        }

        public void OpenWrite()
        {
            writer = new StreamWriter(fullPath,
                new FileStreamOptions { Access = FileAccess.Write, Mode = FileMode.Create, BufferSize = BufferSize });
        }

        public void Close()
        {
            writer?.Close();
            reader?.Close();
        }

        public async Task<TData> GetDataAsync()
        {
            if (IsEnd())
            {
                return default;
            }
            
            var load = await reader.ReadLineAsync();
            return (TData)itemFactory.Create(typeof(TData), load);
        }

        public Task<long> GetBulkDataAsync(TData[] buffer)
        {
            long index = 0;
            for (index = 0; index < buffer.Length; index++)
            {
                if (IsEnd())
                {
                    return Task.FromResult(index);
                }
                
                var load = reader.ReadLine();
                buffer[index] = (TData)itemFactory.Create(typeof(TData), load);
            }

            return Task.FromResult(index);
        }

        public Task WriteDataAsync(TData data, CancellationToken cancellationToken)
        {
            return writer.WriteLineAsync(data.ToString());
        }
        
        public Task WriteBulkDataAsync(TData[] data, int start, long length, CancellationToken cancellationToken)
        {
            var builder = new StringBuilder();
            for (long index = start; index < length; index++)
            {
                builder.AppendLine(data[index].ToString());
            }
            
            return writer.WriteAsync(builder, cancellationToken);
        }

        public bool IsEnd()
        {
            return reader?.EndOfStream ?? false;
        }

        public long Length()
        {
            return reader?.BaseStream.Length ?? 0;
        }

        public void Dispose()
        {
            Close();
        }
    }
}