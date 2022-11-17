namespace Sorter.Logic
{
    using System;

    internal sealed class FileStoreReadConversion : FileStore
    {
        private readonly Func<string, string> convert;

        public FileStoreReadConversion(string path, string name, Func<string,string> convert) : base(path, name)
        {
            this.convert = convert;
        }

        protected override string ConvertInput(string input) => convert(input);
    }
}