namespace Phraser
{
    /// <summary>
    /// Selects a random value item to be returned with <see cref="Phrase{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RandomPhraseValueSelector<T> : IPhraseValueSelector<T>
    {
        public RandomPhraseValueSelector() : this(new Random()) { }
        public RandomPhraseValueSelector(Random rnd)
        {
            Rnd = rnd;
        }

        public Random Rnd { get; }

        public T Select(List<T> values)
        {
            return values[Rnd.Next(values.Count)];
        }
    }
}