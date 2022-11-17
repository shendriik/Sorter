namespace Sorter.Contracts
{
    internal interface IDataStoreBuilder<TData>
    {
        IDataStore<TData> Build(string name);

        IDataStore<TData> ReadConversionBuild(string name);

        IDataStore<TData> WriteConversionBuild(string name);
    }
}