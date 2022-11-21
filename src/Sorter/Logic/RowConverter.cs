namespace Sorter.Logic
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Microsoft.Extensions.Options;

    internal sealed class RowConverter : IDataConverter<string>
    {
        private readonly Settings settings;
        private const char ZeroChar = (char)0x30;
        private const char EmptyChar = (char)0x01;
        private const char DotChar = '.';
        private const int limit = 1000000;

        private int maxDigitLength;

        public RowConverter(IOptions<Settings> settings)
        {
            this.settings = settings.Value;
        }
        
        public async Task ConfigureFromSourceAsync(IDataStore<string> src)
        {
            var maxNumberSize = 0;

            if (settings.MaxDigitLength > 0)
            {
                this.maxDigitLength = settings.MaxDigitLength;
                return;
            }
            
            for (var index = 0; index < limit && !src.IsEnd(); index++)
            {
                var row = await src.GetDataAsync();
                if (row == null)
                {
                    break;
                }

                var dotIndex = row.IndexOf(DotChar, StringComparison.Ordinal);

                maxNumberSize = Math.Max(maxNumberSize, dotIndex);
            }

            maxDigitLength = maxNumberSize;
        }

        public string InputConversion(string input)
        {
            var dotIndex = input.IndexOf(DotChar, StringComparison.Ordinal);

            return $"{input[(dotIndex + 2)..]}{EmptyChar}{DotChar}{NormalizeNumber(input[..dotIndex])}";
        }

        public string OutputConversion(string output)
        {
            var textEndPosition = output.IndexOf(EmptyChar, StringComparison.Ordinal);

            var number = output[(textEndPosition + 2)..];
            var text = output[..textEndPosition];

            return $"{TrimNumber(number)}{DotChar} {text}";
        }

        private string NormalizeNumber(string src)
        {
            return new string(ZeroChar, maxDigitLength - src.Length) + src;
        }

        private static string TrimNumber(string src)
        {
            return src.TrimStart(ZeroChar);
        }
    }
}