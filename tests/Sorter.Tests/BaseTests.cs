namespace Sorter.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Contracts;
    using NSubstitute;

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

        protected static IComparer<T> GetDefaultComparer<T>()
        {
            var comparer = Substitute.For<IComparer<T>>();
            
            comparer.Compare(Arg.Any<T>(), Arg.Any<T>())
                .Returns(x => Comparer<T>.Default.Compare(x.ArgAt<T>(0), x.ArgAt<T>(1)));

            return comparer;
        }
    }    
}
