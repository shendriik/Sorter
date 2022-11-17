namespace Sorter.Contracts
{
    using System.Threading.Tasks;

    internal interface IDataConverter<TData>
    {
        int DataSize { get; }
        
        Task ConfigureFromSourceAsync(IDataStore<TData> src);
            
        TData InputConversion(TData input);
        
        TData OutputConversion(TData output);
    }
}