namespace Sorter.Logic
{
    using System;
    using System.Collections.Generic;

    internal sealed class StringDefaultComparer : Comparer<string>
    {
        public override int Compare(string str1, string str2)
        {
            return string.CompareOrdinal(str1, str2);
        }
    }
}