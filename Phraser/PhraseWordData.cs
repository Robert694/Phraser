namespace Phraser
{
    public class PhraseWordData<T>
    {
        public Dictionary<string, PhraseWordData<T>> Next { get; set; }
        public List<T> Values { get; set; }

        public void AddValue(T value)
        {
            Values ??= new List<T>();
            Values.Add(value);
        }
    }
}