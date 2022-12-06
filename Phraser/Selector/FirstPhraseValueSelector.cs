namespace Phraser
{
    public class FirstPhraseValueSelector<T> : IPhraseValueSelector<T>
    {
        public T Select(List<T> values)
        {
            return values[0];
        }
    }
}