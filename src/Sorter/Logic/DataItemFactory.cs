namespace Sorter.Logic
{
    using System;
    using Contracts;
    using Shared.Models;

    internal sealed class DataItemFactory : IDataItemFactory
    {
        public BaseDataItem Create(Type type, string load)
        {
            if( type == typeof(DataItem))
            {
                return new DataItem(load);
            }

            throw new NotSupportedException($"Unsupported item type {type}");
        }
    }
}