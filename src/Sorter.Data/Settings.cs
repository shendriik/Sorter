namespace Sorter.Data
{
    public sealed class Settings
    {
        public long SizeInBytes { get; set; }
        
        public string Path { get; set; }

        public int DuplicateEachLineNumber { get; set; }

        public int MinWordsCount { get; set; }

        public int MaxWordsCount { get; set; }
        
        public int MaxNumber { get; set; }
    }
}