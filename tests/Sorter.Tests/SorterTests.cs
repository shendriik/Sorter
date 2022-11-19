namespace Sorter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Logic;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NUnit.Framework;

    internal sealed class SorterTests : BaseTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return CreateLongSequenceCase(1000, 100);
            yield return CreateLongSequenceCase(1000, 1000);
            yield return CreateLongSequenceCase(1000, 2000);
            yield return CreateLongSequenceCase(100000, 10000);
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task<string[]> Should_sort_strings_functional(string[] input, int bufferSizeKb, int lineSize)
        {
            // Given
            var merger = new KWayMerger<string>(_ => GetDefaultComparer<string>());
            var settings = Options.Create(new Settings { MergeDeep = 10, BufferSizeKb = bufferSizeKb });

            var converter = Substitute.For<IDataConverter<string>>();
            converter.DataSize.Returns(lineSize);
            
            var storeBuilder = Substitute.For<IDataStoreBuilder>();
            storeBuilder.Build(default, default, default).ReturnsForAnyArgs(x => new MemoryTestDataStore<string>());
            
            var instance = new Sorter(
                converter, 
                settings, 
                storeBuilder,
                _ => GetDefaultComparer<string>(),
                merger);
            
            var output = new MemoryTestDataStore<string>();
            
            // When
            await instance.SortAsync(
                await ArrayToDataStoreAsync(input),
                output);

            // Then
            return await DataStoreToArrayAsync(output);
        }

        private static TestCaseData CreateLongSequenceCase(int size, int bufferSize)
        {
            var sequence = new string[size];
            for (var index = 0; index < sequence.Length; index++)
            {
                sequence[index] = Guid.NewGuid().ToString();
            }
            
            var lineSize = sequence[0].Length;
            var bufferSizeKb = bufferSize * lineSize / 1024;

            var sorted = sequence.ToArray();
            Array.Sort(sorted);
            
            return new TestCaseData(sequence, bufferSizeKb, lineSize) { ExpectedResult = sorted };
        }
    }    
}
