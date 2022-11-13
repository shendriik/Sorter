namespace Sorter.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class BufferedMerger: IMerger
    {
        private const int ChunkSize = 3 * 1024 * 1024;
        
        public async Task MergeAsync<TData>(
            IDataStore<TData> src1, 
            IDataStore<TData> src2, 
            IDataStore<TData> dest, 
            CancellationToken cancellationToken = default) 
            where TData : IComparable<TData>
        {
            var buff1 = new TData[ChunkSize]; 
            var buff2 = new TData[ChunkSize];
            var output = new TData[2 * ChunkSize];

            long size1 = 0;
            long size2 = 0;
            long read1 = 0;
            long read2 = 0;
            long write = 0; 
            
            while (true)
            {
                TData writeData;
                
                if (size1 == 0)
                {
                    size1 = await GetDataFrom(src1, buff1);
                    read1 = 0;
                }
                
                if (size2 == 0)
                {
                    size2 = await GetDataFrom(src2, buff2);
                    read2 = 0;
                }
                
                if (size1 == 0 && size2 == 0)
                {
                    if (write > 0)
                    {
                        await dest.WriteBulkDataAsync(output, 0, write, cancellationToken);
                    }

                    break;
                }

                if (size1 == 0)
                {
                    writeData = buff2[read2];
                    read2++;
                    size2--;
                }
                else if (size2 == 0)
                {
                    writeData = buff1[read1];
                    read1++;
                    size1--;
                }
                else if (buff1[read1].CompareTo(buff2[read2]) <= 0)
                {
                    writeData = buff1[read1];
                    read1++;
                    size1--;
                }
                else
                {
                    writeData = buff2[read2];
                    read2++;
                    size2--;
                }

                output[write++] = writeData;
                
                if (write == output.Length || (size1 == 0 && size2 == 0))
                {
                    await dest.WriteBulkDataAsync(output, 0, write, cancellationToken);
                    write = 0;
                }
            }

            src1?.Close();
            src2?.Close();
            dest.Close();
        }
        
        private async Task<long> GetDataFrom<TData>(IDataStore<TData> src, TData[] buffer)
        {
            if (src == null)
            {
                return 0;
            }

            return await src.GetBulkDataAsync(buffer);
        }
    }
}