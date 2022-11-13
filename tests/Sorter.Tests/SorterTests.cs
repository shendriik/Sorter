namespace Sorter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts;
    using Logic;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using NUnit.Framework;
    using Shared.Models;

    internal sealed class SorterTests : BaseTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(arg: Array.Empty<(int, string)>())
            {
                ExpectedResult = Array.Empty<int>()
            };
            yield return new TestCaseData(arg: new[] { (1,"A") })
            {
                ExpectedResult = new[] { (1,"A") }
            };
            yield return new TestCaseData(arg: new[] { (1,"C"), (1,"A"), (1,"B"), (2,"A") })
            {
                ExpectedResult = new[] { (1, "A"), (2, "A"), (1,"B"), (1,"C") }
            };
            yield return CreateLongSequenceCase();
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task<(int, string)[]> Should_sort_data((int Number, string Text)[] input)
        {
            // Given
            var merger = new Merger(); // must be mocked
            var logger = Substitute.For<ILogger<Sorter<DataItem>>>();
            var storeBuilder = Substitute.For<IDataStoreBuilder>();
            storeBuilder.Build<DataItem>(Arg.Any<string>()).Returns(x => new MemoryTestDataStore<DataItem>());

            var output = new MemoryTestDataStore<DataItem>();
            var instance = new Sorter<DataItem>(merger, storeBuilder, logger);
            
            // When
            await instance.SortAsync(
                await ArrayToDataStoreAsync(input.Select(x => new DataItem { Number = x.Number, Text = x.Text}).ToArray()),
                output);

            // Then
            var itemsResult = await DataStoreToArrayAsync(output);
            var result = itemsResult.Select(x => (x.Number, x.Text)).ToArray();

            return result;
        }

        private static TestCaseData CreateLongSequenceCase()
        {
            var sequence = new (int Number, string Text)[100000];
            var random = new Random();
            for (var index = 0; index < sequence.Length; index++)
            {
                sequence[index] = (random.Next(10000) + 1, Guid.NewGuid().ToString());
            }

            var sorted = sequence.Select(s => new DataItem { Number = s.Number, Text = s.Text }).ToArray();
            Array.Sort(sorted);
            
            return new TestCaseData(arg: sequence) { ExpectedResult = sorted.Select(x => (x.Number, x.Text))};
        }
    }    
}
