namespace Sorter.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Logic;
    using NSubstitute;
    using NUnit.Framework;

    internal sealed class MergerTests : BaseTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                new[] { 7, 8, 9, 10 },
                new[] { 1, 3, 5, 6 },
                new[] { 0, 2, 4, 11 },
                new[] { 1, 12, 13, 15 })
            {
                ExpectedResult = new[]
                {
                    0, 1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 15
                }
            };

            yield return new TestCaseData(
                new[] { "b", "c", "d", "e" },
                new[] { "a", "aa", "bb", "ee" },
                new[] { "abc", "abd", "abe", "baa" },
                new[] { "aaa", "bbb", "ccc", "ddd" })
            {
                ExpectedResult = new[]
                {
                    "a", "aa", "aaa", "abc", "abd", "abe", "b", "baa", "bb", "bbb", "c", "ccc", "d", "ddd", "e", "ee"
                }
            };
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task<T[]> Should_merge_sorted_data<T>(T[] source1,T[] source2, T[] source3, T[] source4)
        {
            // Given
            var comparer = Substitute.For<IComparer<T>>();
            comparer.Compare(Arg.Any<T>(), Arg.Any<T>()).Returns(x =>
                Comparer<T>.Default.Compare(x.ArgAt<T>(0), x.ArgAt<T>(1)));
            
            var instance = new KWayMerger<T>(_ => comparer);
            var output = new MemoryTestDataStore<T>();
            
            // When
            await instance.MergeAsync(
                new []
                {
                    await ArrayToDataStoreAsync(source1),
                    await ArrayToDataStoreAsync(source2),
                    await ArrayToDataStoreAsync(source3),
                    await ArrayToDataStoreAsync(source4),
                },
                output);
            
            // Then
            return await DataStoreToArrayAsync(output);
        }
    }    
}
