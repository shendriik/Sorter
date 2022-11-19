namespace Sorter.Logic.Files
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class FileStore: IDataStore<string>
    {
        private const string FileNameFormat = "part{0}.txt";
        private const int BufferSize = 80 * 1024;

        private readonly Func<string, string> inputConvert;
        private readonly Func<string, string> outputConvert;
            
        private readonly string fullPath;
        private StreamReader reader;
        private StreamWriter writer;

        public FileStore(string path, 
            string name, 
            Func<string,string> inputConvert,
            Func<string,string> outputConvert)
        {
            var fileName = string.IsNullOrEmpty(Path.GetExtension(name))
                ? string.Format(FileNameFormat, name)
                : name;
            
            fullPath = Path.Combine(path, fileName);

            this.inputConvert = inputConvert;
            this.outputConvert = outputConvert;
        }

        public void OpenRead()
        {
            reader = new StreamReader(fullPath,
                new FileStreamOptions
                {
                    Access = FileAccess.Read,
                    Mode = FileMode.Open,
                    BufferSize = BufferSize
                });
        }

        public void OpenWrite()
        {
            writer = new StreamWriter(fullPath,
                new FileStreamOptions
                {
                    Access = FileAccess.Write,
                    Mode = FileMode.Create,
                    BufferSize = BufferSize
                });
        }

        public async Task<string> GetDataAsync()
        {
            return IsEnd() ? null : ConvertInput(await reader.ReadLineAsync());
        }

        public async Task<long> GetBulkDataAsync(string[] buffer)
        {
            long index = 0;
            for (index = 0; index < buffer.Length && !IsEnd(); index++)
            {
                var load = await reader.ReadLineAsync();
                buffer[index] = ConvertInput(load);
            }

            return index;
        }

        public Task WriteDataAsync(string data, CancellationToken cancellationToken)
        {
            return writer.WriteLineAsync(ConvertOutput(data).AsMemory(), cancellationToken);
        }

        public bool IsEnd()
        {
            return reader.EndOfStream;
        }

        public long Length()
        {
            return reader.BaseStream.Length;
        }

        public void Close()
        {
            writer?.Close();
            reader?.Close();
        }

        public void Dispose()
        {
            Close();
        }

        private string ConvertInput(string input) => inputConvert(input);

        private string ConvertOutput(string output) => outputConvert(output);
    }
}