namespace Phraser
{
    public interface IPhraseParser<T>
    {
        public int PhraseCount { get; }
        public void LoadPhrases(IPhraseSupplier<T> loader);
        public IEnumerable<Phrase<T>> Parse(string[] input, IPhraseValueSelector<T> selector);
    }
}