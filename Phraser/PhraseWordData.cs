namespace Phraser
{
    public class PhraseWordData<T>
    {
        public Dictionary<string, PhraseWordData<T>> Next { get; set; }
        public List<T> Values { get; set; }
    }
}