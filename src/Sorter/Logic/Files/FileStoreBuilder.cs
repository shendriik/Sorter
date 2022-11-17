namespace Sorter.Logic
{
    using Contracts;
    using Microsoft.Extensions.Options;

    internal sealed class FileStoreBuilder: IDataStoreBuilder<string>
    {
        private readonly IDataConverter<string> converter;
        private readonly Settings settings;

        public FileStoreBuilder(IOptions<Settings> settings, IDataConverter<string> converter)
        {
            this.settings = settings.Value;
            this.converter = converter;
        }

        public IDataStore<string> Build(string name)
        {
            return new FileStore(settings.Path, name);
        }
        
        public IDataStore<string> ReadConversionBuild(string name)
        {
            return new FileStoreReadConversion(settings.Path, name, converter.InputConversion);
        }
        
        public IDataStore<string> WriteConversionBuild(string name)
        {
            return new FileStoreWriteConversion(settings.Path, name, converter.OutputConversion);
        }
    }
}