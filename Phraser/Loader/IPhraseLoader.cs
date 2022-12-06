using System.IO;

namespace Phraser
{
    public interface IPhraseLoader<T>
    {
        /// <summary>
        /// Loads phrases from user defined source
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PhraseLoaderData<T>> GetPhrases();
    }
}