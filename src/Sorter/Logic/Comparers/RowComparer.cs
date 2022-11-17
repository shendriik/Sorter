namespace Sorter.Logic
{
    using System;
    using System.Collections.Generic;

    internal sealed class RowComparer : Comparer<string>
    {
        public override int Compare(string str1, string str2)
        {
            var dotIndex1 = str1.IndexOf(".", StringComparison.Ordinal);
            var dotIndex2 = str2.IndexOf(".", StringComparison.Ordinal);

            var compare = string.CompareOrdinal(str1[(dotIndex1 + 2)..], str2[(dotIndex2 + 2)..]);
            if (compare != 0)
            {
                return compare;
            }

            var numbersLengthDiff = dotIndex1 - dotIndex2;
            
            return numbersLengthDiff == 0
                ? string.CompareOrdinal(str1[..dotIndex1], str2[..dotIndex2])
                : numbersLengthDiff;
        }
    }
}