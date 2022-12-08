namespace Phraser
{
    /// <summary>
    /// Loads phrases from text file
    /// </summary>
    public class PhraseFileSupplier : IPhraseSupplier<string>
    {
        public PhraseFileSupplier(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }

        public IEnumerable<PhraseSupplierData<string>> GetPhrases()
        {
            if (!File.Exists(FilePath)) throw new FileNotFoundException(FilePath);
            foreach (string line in File.ReadLines(FilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var phrase = PhraseParser<string>.Sanitize(Path.GetFileNameWithoutExtension(line).ToUpper().Split(' '));
                if (phrase.Length == 0) continue;
                yield return new PhraseSupplierData<string>(phrase, line + ".mp3");
            }
        }
    }
}