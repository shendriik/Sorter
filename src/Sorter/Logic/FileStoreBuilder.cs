namespace Sorter.Logic
{
    using Contracts;
    using Microsoft.Extensions.Options;
    using Shared.Models;

    internal sealed class FileStoreBuilder: IDataStoreBuilder
    {
        private readonly IDataItemFactory dataItemFactory;
        private readonly Settings settings;

        public FileStoreBuilder(IDataItemFactory dataItemFactory, IOptions<Settings> settings)
        {
            this.dataItemFactory = dataItemFactory;
            this.settings = settings.Value;
        }
        public IDataStore<TData> Build<TData>(string name) where TData: BaseDataItem
        {
            return new FileStore<TData>(settings.Path, name, dataItemFactory);
        }
    }
}