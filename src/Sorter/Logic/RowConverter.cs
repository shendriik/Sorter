namespace Sorter.Logic
{
    using System;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class RowConverter : IDataConverter<string>
    {
        private const char EmptyChar = (char)0x01;
        private int maxTextLength;
        private int maxDigitLength;

        public int DataSize => maxDigitLength + maxTextLength;

        public async Task ConfigureFromSourceAsync(IDataStore<string> src)
        {
            var maxNumberSize = 0;
            var maxTextSize = 0;

            while (true)
            {
                var row = await src.GetDataAsync();
                if (row == null)
                {
                    break;
                }

                var dotIndex = row.IndexOf(".", StringComparison.Ordinal);

                if (maxNumberSize < dotIndex)
                {
                    maxNumberSize = dotIndex;
                }

                if (maxTextSize < row.Length - dotIndex - 1)
                {
                    maxTextSize = row.Length - dotIndex - 1;
                }
            }

            ConfigureInternal(maxTextSize, maxNumberSize);
        }

        public string InputConversion(string input)
        {
            var dotIndex = input.IndexOf(".", StringComparison.Ordinal);

            return NormalizeText(input[(dotIndex + 2)..]) + "." + NormalizeNumber(input[..dotIndex]);
        }

        public string OutputConversion(string output)
        {
            var textEndPosition = output.IndexOf(EmptyChar, StringComparison.Ordinal);

            var number = output[(maxTextLength + 1)..];
            var text = output[..textEndPosition];

            return TrimNumber(number) + ". " + text;
        }

        private string NormalizeNumber(string src)
        {
            return new string((char)0x30, maxDigitLength - src.Length) + src;
        }

        private string NormalizeText(string src)
        {
            return src + new string(EmptyChar, maxTextLength - src.Length);
        }

        private static string TrimNumber(string src)
        {
            return src.TrimStart((char)0x30);
        }

        private void ConfigureInternal(int maxTextSize, int maxDigitSize)
        {
            maxTextLength = maxTextSize + 1; // hack to make find end of the text instead of searching "."
            maxDigitLength = maxDigitSize;
        }
    }
}