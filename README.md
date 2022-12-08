# Phraser
A phrase parsing library

![Build status](https://github.com/Robert694/Phraser/actions/workflows/dotnet_package.yml/badge.svg)

Example
```cs
using Phraser;

//Initialize phrase parser
IPhraseParser<string> parser = new PhraseParser<string>();
//Define where phrases will be supplied from
IPhraseSupplier<string> loader = new FileNamePhraseSupplier(Path.Combine(Environment.CurrentDirectory, "Sounds"), new[] { ".wav", ".ogg", ".mp3", ".mid" });
//Define how a phrase value will be selected
IPhraseValueSelector<string> selector = new RandomPhraseValueSelector<string>();
//Load phrases into phrase parser
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
```
In the above example if the following filepaths existed:
+ Sounds/hello.mp3
+ Sounds/hello hello.mp3
+ Sounds/mika.mp3

Given the input "Hello hello hello Mika cat" the result would be: 
```
Phrases: 3
Input: Hello hello hello Mika cat
[0..2] 'Hello hello' = hello hello.mp3
[2..3] 'hello' = hello.mp3
[3..4] 'Mika' = mika.mp3
[4..5] 'cat' =
```
