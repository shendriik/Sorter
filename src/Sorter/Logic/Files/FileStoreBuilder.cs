namespace Sorter.Logic.Files
{
    using Contracts;
    using Microsoft.Extensions.Options;

    internal sealed class FileStoreBuilder: IDataStoreBuilder
    {
        private readonly IDataConverter<string> converter;
        private readonly Settings settings;

        public FileStoreBuilder(IOptions<Settings> settings, IDataConverter<string> converter)
        {
            this.settings = settings.Value;
            this.converter = converter;
        }
        
        public IDataStore<string> Build(IDataStore<string> store, string[] buffer, int start, int size)
        {
            return new BufferedFileStore(store, buffer, start, size);
        }
        
        public IDataStore<string> Build(string name, bool readConvert = false, bool writeConvert = false)
        {
            return new FileStore(
                settings.Path,
                name,
                readConvert ? converter.InputConversion : s => s,
                writeConvert ? converter.OutputConversion : s => s);
        }
    }
}