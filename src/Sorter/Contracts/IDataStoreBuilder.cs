namespace Sorter.Contracts
{
    internal interface IDataStoreBuilder
    {
        IDataStore<string> Build(IDataStore<string> store);

        IDataStore<string> Build(string name, bool readConvert = false, bool writeConvert = false);
    }
}