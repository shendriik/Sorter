// namespace Sorter.Tests
// {
//     using System;
//     using System.Collections.Generic;
//     using System.Linq;
//     using System.Threading.Tasks;
//     using Contracts;
//     using Logic;
//     using Microsoft.Extensions.Logging;
//     using NSubstitute;
//     using NUnit.Framework;
//     using Shared.Models;
//
//     internal sealed class SorterTests : BaseTests
//     {
//         public static IEnumerable<TestCaseData> TestCases()
//         {
//             yield return CreateLongSequenceCase();
//         }
//
//         [TestCaseSource(nameof(TestCases))]
//         public async Task<(int, string)[]> Should_sort_data((int Number, string Text)[] input)
//         {
//             // Given
//             var merger = new Merger(); // must be mocked
//             var logger = Substitute.For<ILogger<Sorter<DataItem>>>();
//             var storeBuilder = Substitute.For<IDataStoreBuilder>();
//             storeBuilder.Build<DataItem>(Arg.Any<string>()).Returns(x => new MemoryTestDataStore<DataItem>());
//
//             var output = new MemoryTestDataStore<DataItem>();
//             var instance = new Sorter<DataItem>(merger, storeBuilder, logger);
//             
//             // When
//             await instance.SortAsync(
//                 await ArrayToDataStoreAsync(input.Select(x => new DataItem { Number = x.Number, Text = x.Text}).ToArray()),
//                 output);
//
//             // Then
//             var itemsResult = await DataStoreToArrayAsync(output);
//             var result = itemsResult.Select(x => (x.Number, x.Text)).ToArray();
//
//             return result;
//         }
//
//         private static TestCaseData CreateLongSequenceCase()
//         {
//             var sequence = new string[100000];
//             for (var index = 0; index < sequence.Length; index++)
//             {
//                 sequence[index] = Guid.NewGuid().ToString();
//             }
//
//             var sorted = sequence.ToArray();
//             Array.Sort(sorted);
//             
//             return new TestCaseData(arg: sequence) { ExpectedResult = sorted))};
//         }
//     }    
// }
