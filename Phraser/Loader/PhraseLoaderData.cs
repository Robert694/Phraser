namespace Phraser
{
    public class PhraseLoaderData<T>
    {
        public PhraseLoaderData(string[] phrase, T value)
        {
            Phrase = phrase;
            Value = value;
        }

        public string[] Phrase { get; set; }
        public T Value { get; set; }
    }
}