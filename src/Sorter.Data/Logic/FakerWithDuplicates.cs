namespace Sorter.Data.Logic
{
    using System;
    using Bogus;
    using Contracts;
    using Microsoft.Extensions.Options;

    internal sealed class FakerWithDuplicates<T> : IFaker<T> where T: class
    {
        private readonly Settings settings;
        private readonly Faker<T> faker;
        
        private readonly int rowNumberDivider;
        private readonly int additionalWordsMax;
        
        private string duplicate;
        
        public FakerWithDuplicates(IOptions<Settings> settings)
        {
            this.settings = settings.Value;
            
            rowNumberDivider = this.settings.DuplicateEachLineNumber - 1;
            additionalWordsMax = this.settings.MaxWordsCount - this.settings.MinWordsCount;
            
            faker = new Faker<T>()
                .RuleForType(typeof(string), SetRuleForStrings)
                .RuleForType(typeof(int), f => f.Random.Int(1, this.settings.MaxNumber));
        }
        
        public T Generate() => faker.Generate();

        private string SetRuleForStrings(Faker f)
        {
            if (f.IndexFaker == 0 || f.IndexFaker % rowNumberDivider != 0)
            {
                return CreateSentence(f);
            }

            if (duplicate == null)
            {
                duplicate = CreateSentence(f);
                return duplicate;
            }

            var output = duplicate;
            duplicate = null;
            return output;
        }

        private string CreateSentence(Faker f)
        {
            var count = settings.MinWordsCount + f.Random.Number(additionalWordsMax);
            
            var sentence = string.Join(" ", f.Lorem.Words(count));
            return string.Concat(sentence.Substring(0, 1).ToUpper(), sentence.AsSpan(1));
        }
    }
}