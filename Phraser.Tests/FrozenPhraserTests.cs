﻿using FluentAssertions;

namespace Phraser.Tests
{
    public class FrozenPhraserTests
    {
        [Theory]
        [InlineData("hello hello hello world", new[] { "hello hello", "hello", "world" })]
        [InlineData("hello hello test hello world", new[] { "hello hello", null, "hello", "world" })]
        [InlineData("hello world", new[] { "hello", "world" })]
        public void Parse_ShouldBeExpectedResult(string input, string[] expectedResult)
        {
            IPhraseParser<string> parser = new FrozenPhraseParser<string>();
            IPhraseSupplier<string> loader = new MemoryPhraseSupplier<string>(
        [
            new(["hello"], "hello"),
            new(["hello", "hello"], "hello hello"),
            new(["world"], "world")
        ]);
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
            IPhraseParser<string> parser = new FrozenPhraseParser<string>();
            IPhraseSupplier<string> loader = new MemoryPhraseSupplier<string>(
        [
            new(["hello"], "hello"),
            new(["hello", "hello"], "hello hello"),
            new(["world"], "world")
        ]);
            IPhraseValueSelector<string> selector = new RandomPhraseValueSelector<string>();
            parser.LoadPhrases(loader);
            parser.Parse(input.Split(), selector)
                .Select(x => x.Value)
                .Should()
                .NotEqual(expectedResult);
        }
    }
}
