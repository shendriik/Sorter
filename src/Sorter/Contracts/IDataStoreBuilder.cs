namespace Sorter.Contracts
{
    using Shared.Models;

    internal interface IDataStoreBuilder
    {
        IDataStore<TData> Build<TData>(string name) where TData : BaseDataItem;
    }
}