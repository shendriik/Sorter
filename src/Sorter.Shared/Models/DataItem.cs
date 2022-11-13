namespace Sorter.Shared.Models
{
    using System;
    using System.Collections.Generic;

    public sealed class DataItem : BaseDataItem, IComparable<DataItem>
    {
        public DataItem()
        {
            
        }
        
        public DataItem(string load)
        {
            Number = int.Parse(load.Substring(0, load.IndexOf(".", StringComparison.Ordinal)));
            Text = load.Substring(load.IndexOf(" ", StringComparison.Ordinal) + 1);
        }
        
        public string Text { get; set; }
        
        public int Number { get; set; }

        public int CompareTo(DataItem other)
        {
            var compare = string.Compare(Text, other.Text, StringComparison.Ordinal);
            
            return compare == 0 
                ? Comparer<int>.Default.Compare(Number, other.Number)
                : compare;
        }

        public override string ToString()
        {
            return $"{Number}. {Text}";
        }
    }
}