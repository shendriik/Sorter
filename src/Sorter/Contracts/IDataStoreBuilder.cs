namespace Sorter.Contracts
{
    internal interface IDataStoreBuilder
    {
        IDataStore<string> Build(IDataStore<string> store, string[] buffer, int start, int size);

        IDataStore<string> Build(string name, bool readConvert = false, bool writeConvert = false);
    }
}