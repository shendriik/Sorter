namespace Sorter
{
    public sealed class Settings
    {
        public string Path { get; set; }
        
        public string SourceFileName { get; set; }
        
        public string DestinationFileName { get; set; }
        
        public int StringBufferLength { get; set; }
        
        public int MaxDigitLength { get; set; }
    }
}