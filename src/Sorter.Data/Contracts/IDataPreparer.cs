namespace Sorter.Data.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    internal interface IDataPreparer
    {
        Task ExecuteAsync(CancellationToken cancellation);
    }
}
