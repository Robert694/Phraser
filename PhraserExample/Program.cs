using Phraser;

IPhraseParser<string> parser = new PhraseParser<string>();
IPhraseSupplier<string> loader = new FileNamePhraseSupplier(Path.Combine(Environment.CurrentDirectory, "Sounds"), new[] { ".wav", ".ogg", ".mp3", ".mid" });
IPhraseValueSelector<string> selector = new RandomPhraseValueSelector<string>();
parser.LoadPhrases(loader);
Console.WriteLine($"Phrases: {parser.PhraseCount}");

while (true)
{
    Console.Write("Input: ");
    var line = Console.ReadLine();
    if (line == null) continue;
    var input = PhraseParser<string>.Sanitize(line.Split());
    foreach (var phrase in parser.Parse(input, selector))
    {
        if (phrase.Found) Console.ForegroundColor = ConsoleColor.Green; else Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{phrase.Range}] '{string.Join(" ", input[phrase.Range])}' = {Path.GetFileName(phrase.Value)}");
        Console.ResetColor();
    }
}