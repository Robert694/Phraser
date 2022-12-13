using FluentAssertions;

namespace Phraser.Tests
{
    public class PhraserTests
    {
        [Theory]
        [InlineData("hello hello hello world", new[] {"hello hello", "hello", "world"})]
        [InlineData("hello hello test hello world", new[] { "hello hello", null, "hello", "world" })]
        [InlineData("hello world", new[] { "hello", "world" })]
        public void Parse_ShouldBeExpectedResult(string input, string[] expectedResult)
        {
            IPhraseParser<string> parser = new PhraseParser<string>();
            IPhraseSupplier<string> loader = new MemoryPhraseSupplier<string>(new List<PhraseSupplierData<string>>()
        {
            new PhraseSupplierData<string>(new[]{"hello"}, "hello"),
            new PhraseSupplierData<string>(new[]{"hello","hello"}, "hello hello"),
            new PhraseSupplierData<string>(new[]{"world"}, "world")
        });
            IPhraseValueSelector<string> selector = new RandomPhraseValueSelector<string>();
            parser.LoadPhrases(loader);
            parser.Parse(input.Split(), selector)
                .Select(x => x.Value)
                .Should()
                .Equal(expectedResult);
        }

        [Theory]
        [InlineData("hello hello hello world", new[] { "hello", "hello", "hello", "world" })]
        [InlineData("hello hello test hello world", new[] { "hello hello", "test", "hello", "world" })]
        [InlineData("hello world", new[] { "hello world" })]
        public void Parse_ShouldNotBeExpectedResult(string input, string[] expectedResult)
        {
            IPhraseParser<string> parser = new PhraseParser<string>();
            IPhraseSupplier<string> loader = new MemoryPhraseSupplier<string>(new List<PhraseSupplierData<string>>()
        {
            new PhraseSupplierData<string>(new[]{"hello"}, "hello"),
            new PhraseSupplierData<string>(new[]{"hello","hello"}, "hello hello"),
            new PhraseSupplierData<string>(new[]{"world"}, "world")
        });
            IPhraseValueSelector<string> selector = new RandomPhraseValueSelector<string>();
            parser.LoadPhrases(loader);
            parser.Parse(input.Split(), selector)
                .Select(x => x.Value)
                .Should()
                .NotEqual(expectedResult);
        }

        [Theory]
        [InlineData("hello hello hello world", new[] { "hello", "hello", "hello", "world" })]
        [InlineData("hello hello      .....       hello world!!!", new[] { "hello", "hello", "hello", "world" })]
        public void Sanitize_ShouldBeExpectedResult(string input, string[] expectedResult)
        {
            PhraseParser.Sanitize(input.Split())
                .Should()
                .Equal(expectedResult);
        }
    }
}
