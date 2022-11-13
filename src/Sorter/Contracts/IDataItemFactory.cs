namespace Sorter.Contracts
{
    using System;
    using Shared.Models;

    public interface IDataItemFactory
    {
        BaseDataItem Create(Type type, string load);
    }
}