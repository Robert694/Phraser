namespace Phraser
{
    /// <summary>
    /// Loads filesnames as phreases and uses filepath as value
    /// </summary>
    public class FileNamePhraseSupplier : IPhraseSupplier<string>
    {
        public FileNamePhraseSupplier(string directoryPath, string[]? allowedFileTypes = null, bool recursive = true)
        {
            DirectoryPath = directoryPath;
            AllowedFileTypes = allowedFileTypes;
            Recursive = recursive;
        }

        public string DirectoryPath { get; }
        public string[]? AllowedFileTypes { get; }
        public bool Recursive { get; }

        public IEnumerable<PhraseSupplierData<string>> GetPhrases()
        {
            if (!Directory.Exists(DirectoryPath)) throw new DirectoryNotFoundException(DirectoryPath);
            foreach (string file in Directory.EnumerateFiles(DirectoryPath, "*.*", Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (AllowedFileTypes != null && !AllowedFileTypes.Contains(Path.GetExtension(file).ToLower())) continue;//Skip invalid filetype
                var phrase = PhraseParser<string>.Sanitize(Path.GetFileNameWithoutExtension(file).ToUpper().Split(' '));
                if (phrase.Length == 0) continue; 
                yield return new PhraseSupplierData<string>(phrase, file);
            }
        }
    }
}