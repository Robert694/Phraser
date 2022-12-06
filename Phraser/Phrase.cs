namespace Phraser
{
    public class Phrase<T>
    {
        public Phrase(bool found, Range range, T value)
        {
            Found = found;
            Range = range;
            Value = value;
        }

        public bool Found { get; set; }
        public Range Range { get; set; }
        public T Value { get; set; }
    }
}