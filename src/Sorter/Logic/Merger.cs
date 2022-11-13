namespace Sorter.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class Merger: IMerger
    {
        public async Task MergeAsync<TData>(
            IDataStore<TData> src1, 
            IDataStore<TData> src2, 
            IDataStore<TData> dest, 
            CancellationToken cancellationToken = default) 
            where TData : IComparable<TData>
        {
            var data1 = await GetDataFrom(src1);
            var data2 = await GetDataFrom(src2);

            while (!IsNull(data1) || !IsNull(data2))
            {
                TData writeData;
                if (IsNull(data1))
                {
                    writeData = data2;
                    data2 = await GetDataFrom(src2);
                }
                else if (IsNull(data2))
                {
                    writeData = data1;
                    data1 = await GetDataFrom(src1);
                }
                else if (data1.CompareTo(data2) <= 0)
                {
                    writeData = data1;
                    data1 = await GetDataFrom(src1);
                }
                else
                {
                    writeData = data2;
                    data2 = await GetDataFrom(src2);
                }

                await dest.WriteDataAsync(writeData, cancellationToken);
            }

            src1?.Close();
            src2?.Close();
            dest.Close();
        }

        private bool IsNull<TData>(TData obj)
        {
            return EqualityComparer<TData>.Default.Equals(obj, default);
        }
        
        private async Task<TData> GetDataFrom<TData>(IDataStore<TData> src)
        {
            if (src == null)
            {
                return default;
            }

            return await src.GetDataAsync();
        }

    }
}