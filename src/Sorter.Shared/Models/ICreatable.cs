namespace Sorter.Shared.Models
{
    public interface ICreatable<out TData>
    {
        TData Create(string str);
    }
}