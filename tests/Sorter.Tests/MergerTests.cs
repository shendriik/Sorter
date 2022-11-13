namespace Sorter.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Logic;
    using NUnit.Framework;

    internal sealed class MergerTests : BaseTests
    {
        public static IEnumerable<TestCaseData> TestCases()
        {
            yield return new TestCaseData(
                arg1: new[] { 7, 8, 9, 11 },
                arg2: Array.Empty<int>())
            {
                ExpectedResult = new[] { 7, 8, 9, 11 }
            };
            yield return new TestCaseData(
                arg1: new[] { 7, 8, 9, 11 },
                arg2: new[] { 2, 5, 6 })
            {
                ExpectedResult = new[] { 2, 5, 6, 7, 8, 9, 11 }
            };
            yield return new TestCaseData(
                arg1: new[] { 1, 2, 3 },
                arg2: new[] { 2, 5, 6 })
            {
                ExpectedResult = new[] { 1, 2, 2, 3, 5, 6 }
            };
            yield return new TestCaseData(
                arg1: new[] { "a", "c", "e" },
                arg2: new[] { "b", "d", "f" })
            {
                ExpectedResult = new[] { "a", "b", "c", "d", "e", "f" }
            };
            yield return new TestCaseData(
                arg1: new[] { "e" },
                arg2: new[] { "b", "d", "f" })
            {
                ExpectedResult = new[] { "b", "d", "e", "f" }
            };
        }

        [TestCaseSource(nameof(TestCases))]
        public async Task<T[]> Should_merge_sorted_data<T>(T[] src1, T[] src2) where T: IComparable<T>, IEquatable<T>
        {
            // Given
            var instance = new Merger();
            var output = new MemoryTestDataStore<T>();
            
            // When
            await instance.MergeAsync(
                await ArrayToDataStoreAsync(src1),
                await ArrayToDataStoreAsync(src2),
                output);
            
            // Then
            return await DataStoreToArrayAsync(output);
        }
    }    
}
