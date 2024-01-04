namespace Phraser
{
    public class FirstPhraseValueSelector<T> : IPhraseValueSelector<T>
    {
        public T Select(IList<T> values)
        {
            return values[0];
        }
    }
}