namespace Sorter.Logic
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Contracts;

    internal sealed class RowConverter : IDataConverter<string>
    {
        private const char ZeroChar = (char)0x30;
        private const char EmptyChar = (char)0x01;
        private const char DotChar = '.';
        
        private int maxTextLength;
        private int maxDigitLength;

        private StringBuilder builder;

        public int DataSize => maxDigitLength + maxTextLength + 1;

        public RowConverter()
        {
           //builder = new StringBuilder();
        }
        
        public async Task ConfigureFromSourceAsync(IDataStore<string> src)
        {
            var maxNumberSize = 6;
            var maxTextSize = 150;

            // const int limit = 10000000;
            // for(long index=0 ; index<limit && !src.IsEnd();index++)
            // {
            //     var row = await src.GetDataAsync();
            //     if (row == null)
            //     {
            //         break;
            //     }
            //
            //     var dotIndex = row.IndexOf(DotChar, StringComparison.Ordinal);
            //
            //     maxNumberSize = Math.Max(maxNumberSize, dotIndex);
            //     maxTextSize = Math.Max(maxTextSize, row.Length - dotIndex - 1);
            // }
            
            ConfigureInternal(maxTextSize, maxNumberSize);
        }

        public string InputConversion(string input)
        {
            var dotIndex = input.IndexOf(DotChar, StringComparison.Ordinal);

            return NormalizeText(input[(dotIndex + 2)..]) + DotChar + NormalizeNumber(input[..dotIndex]);

            //return PrepareInput(input, dotIndex);
        }

        private string PrepareInput(string input, int dotIndex)
        {
            builder.Clear();

            var textChars = input[(dotIndex + 2)..];
            var numberChars = input[..dotIndex];

            builder.Append(textChars);

            for (var i = 0; i < maxTextLength - textChars.Length; i++)
            {
                builder.Append(EmptyChar);
            }

            builder.Append(DotChar);

            for (var i = 0; i < maxDigitLength - numberChars.Length; i++)
            {
                builder.Append(ZeroChar);
            }

            builder.Append(numberChars);

            return builder.ToString();
        }

        public string OutputConversion(string output)
        {
            var textEndPosition = output.IndexOf(EmptyChar, StringComparison.Ordinal);

            var number = output[(maxTextLength + 1)..];
            var text = output[..textEndPosition];

            return TrimNumber(number) + ". " + text;

            // return PrepareOutput(number, text);
        }

        private string PrepareOutput(string number, string text)
        {
            builder.Clear();

            var start = false;
            for (var index = 0; index < number.Length; index++)
            {
                if (!start && number[index] == ZeroChar)
                {
                    continue;
                }

                start = true;

                builder.Append(number[index]);
            }

            builder.Append(DotChar);
            builder.Append(EmptyChar);
            builder.Append(text);

            return builder.ToString();
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
            maxTextLength = maxTextSize + 1; // hack to make find end of the text instead of searching '.' char
            maxDigitLength = maxDigitSize;
        }
    }
}