namespace Phraser
{
    public interface IPhraseValueSelector<T>
    {
        /// <summary>
        /// Selects a value item to be returned with <see cref="Phrase{T}"/>
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public T Select(List<T> values);
    }
}