namespace Sorter.Contracts
{
    using System.Threading.Tasks;

    internal interface IDataConverter<TData>
    {
        Task ConfigureFromSourceAsync(IDataStore<TData> src);
            
        TData InputConversion(TData input);
        
        TData OutputConversion(TData output);
    }
}