namespace Phraser
{
    /// <summary>
    /// Loads phrases from memory
    /// </summary>
    public class MemoryPhraseSupplier<T> : IPhraseSupplier<T>
    {
        public MemoryPhraseSupplier(List<PhraseSupplierData<T>> phrases)
        {
            Phrases = phrases;
        }

        public List<PhraseSupplierData<T>> Phrases { get; }

        public IEnumerable<PhraseSupplierData<T>> GetPhrases() => Phrases;
    }
}