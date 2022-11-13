namespace Sorter.Data.Contracts
{
    internal interface IFaker<T> where T: class
    {
        T Generate();
    }
}