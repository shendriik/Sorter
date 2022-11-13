namespace Sorter.Tests
{
    using System.Threading.Tasks;
    using Contracts;

    internal abstract class BaseTests
    {
        protected async Task<IDataStore<T>> ArrayToDataStoreAsync<T>(T[] src)
        {
            var store = new MemoryTestDataStore<T>();
            store.OpenWrite();

            foreach (var item in src)
            {
                await store.WriteDataAsync(item); 
            }
            
            store.Close();

            return store;
        }
        
        protected async Task<T[]> DataStoreToArrayAsync<T>(IDataStore<T> store)
        {
            store.OpenRead();

            var dest = new T[store.Length()];
            var index = 0;
            while (!store.IsEnd())
            {
                dest[index++] = await store.GetDataAsync();
            }
            
            store.Close();

            return dest;
        }
    }    
}
