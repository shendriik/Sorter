namespace Sorter.Logic
{
    using System;

    internal sealed class FileStoreWriteConversion : FileStore
    {
        private readonly Func<string, string> convert;

        public FileStoreWriteConversion(string path, string name, Func<string,string> convert) : base(path, name)
        {
            this.convert = convert;
        }

        protected override string ConvertOutput(string output) => convert(output);
    }
}