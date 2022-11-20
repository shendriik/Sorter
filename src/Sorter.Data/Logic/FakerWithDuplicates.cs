namespace Sorter.Data.Logic
{
    using System;
    using System.Linq;
    using Bogus;
    using Contracts;
    using Microsoft.Extensions.Options;

    internal sealed class FakerWithDuplicates : IFaker
    {
        private readonly Settings settings;
        private readonly Faker faker;
        
        private readonly int rowNumberDivider;
        private readonly int additionalWordsMax;
        
        private string duplicate;
        private long rowNumber;
        
        public FakerWithDuplicates(IOptions<Settings> settings)
        {
            this.settings = settings.Value;
            
            rowNumberDivider = this.settings.DuplicateEachLineNumber - 1;
            additionalWordsMax = this.settings.MaxWordsCount - this.settings.MinWordsCount;

            faker = new Faker();
        }

        public string Generate() => GenerateInternal(faker);

        private string GenerateInternal(Faker f)
        {
            rowNumber++;
            
            var number = f.Random.Int(1, this.settings.MaxNumber);
            if (rowNumber % rowNumberDivider != 0)
            {
                return SetFormat(number, CreateSentence(f));
            }

            if (duplicate == null)
            {
                duplicate = CreateSentence(f);
                return SetFormat(number, duplicate);
            }

            var text = duplicate;
            duplicate = null;
            
            return SetFormat(number, text);
        }

        private string CreateSentence(Faker f)
        {
            var count = settings.MinWordsCount + f.Random.Number(additionalWordsMax);

            // var words = Guid.NewGuid().ToString().Split('-');
            var sentence = string.Join(" ", f.Lorem.Words(count));//words.Take(count)); 
            return string.Concat(sentence[..1].ToUpper(), sentence.AsSpan(1));
        }
        
        private string SetFormat(int number, string text)
        {
            return $"{number}. {text}";
        }
    }
}