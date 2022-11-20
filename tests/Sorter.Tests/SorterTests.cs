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
            yield return CreateLongSequenceCase(1000, 1);
            yield return CreateLongSequenceCase(1000, 100);
            yield return CreateLongSequenceCase(1000, 1000);
            yield return CreateLongSequenceCase(1000, 2000);
            yield return CreateLongSequenceCase(100000, 10000);
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task<string[]> Should_sort_strings_functional(string[] input, int bufferLength)
        {
            // Given
            var merger = new KWayMerger<string>(GetDefaultComparer<string>());
            var settings = Options.Create(new Settings { StringBufferLength = bufferLength});

            var storeBuilder = Substitute.For<IDataStoreBuilder>();
            storeBuilder.Build(Arg.Any<string>()).ReturnsForAnyArgs(x => new MemoryTestDataStore<string>());
            storeBuilder.Build(Arg.Any<IDataStore<string>>(), default, default, default).ReturnsForAnyArgs(x => x[0]);
            
            var instance = new Sorter(
                settings, 
                storeBuilder,
                GetDefaultComparer<string>(),
                merger);
            
            var output = new MemoryTestDataStore<string>();
            
            // When
            await instance.SortAsync(
                await ArrayToDataStoreAsync(input),
                output);

            // Then
            return await DataStoreToArrayAsync(output);
        }

        private static TestCaseData CreateLongSequenceCase(int size, int bufferLength)
        {
            var sequence = new string[size];
            for (var index = 0; index < sequence.Length; index++)
            {
                sequence[index] = Guid.NewGuid().ToString();
            }
            
            var sorted = sequence.ToArray();
            Array.Sort(sorted);
            
            return new TestCaseData(sequence, bufferLength) { ExpectedResult = sorted };
        }
    }    
}
