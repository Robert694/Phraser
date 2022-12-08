namespace Phraser
{
    public class PhraseSupplierData<T>
    {
        public PhraseSupplierData(string[] phrase, T value)
        {
            Phrase = phrase;
            Value = value;
        }

        public string[] Phrase { get; set; }
        public T Value { get; set; }
    }
}