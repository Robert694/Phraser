using System.IO;

namespace Phraser
{
    public interface IPhraseSupplier<T>
    {
        /// <summary>
        /// Loads phrases from user defined source
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PhraseSupplierData<T>> GetPhrases();
    }
}