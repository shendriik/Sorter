namespace Sorter.Logic
{
    using System;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class RowConverter : IDataConverter<string>
    {
        private const char ZeroChar = (char)0x30;
        private const char EmptyChar = (char)0x01;
        private const char DotChar = '.';
        
        private int maxDigitLength;

        public int DataSize => 70;
        
        public async Task ConfigureFromSourceAsync(IDataStore<string> src)
        {
             var maxNumberSize = 0;

            const int limit = 10000000;
            for(long index=0 ; index < limit && !src.IsEnd(); index++)
            {
                var row = await src.GetDataAsync();
                if (row == null)
                {
                    break;
                }
            
                var dotIndex = row.IndexOf(DotChar, StringComparison.Ordinal);
            
                maxNumberSize = Math.Max(maxNumberSize, dotIndex);
            }
            
            ConfigureInternal(maxNumberSize);
        }

        public string InputConversion(string input)
        {
            var dotIndex = input.IndexOf(DotChar, StringComparison.Ordinal);

            return input[(dotIndex + 2)..] + EmptyChar + DotChar + NormalizeNumber(input[..dotIndex]);
        }

        public string OutputConversion(string output)
        {
            var textEndPosition = output.IndexOf(EmptyChar, StringComparison.Ordinal);

            var number = output[(textEndPosition + 2)..];
            var text = output[..textEndPosition];

            return TrimNumber(number) + ". " + text;
        }

        private string NormalizeNumber(string src)
        {
            return new string(ZeroChar, maxDigitLength - src.Length) + src;
        }

        private static string TrimNumber(string src)
        {
            return src.TrimStart(ZeroChar);
        }

        private void ConfigureInternal(int maxDigitSize)
        {
            maxDigitLength = maxDigitSize;
        }
    }
}