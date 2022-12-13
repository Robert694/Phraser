using System.Text.RegularExpressions;

namespace Phraser
{
    public class PhraseParser<T> : IPhraseParser<T>
    {
        public Dictionary<string, PhraseWordData<T>> WordDatas { get; private set; }
        public int PhraseCount { get; private set; }
        public void LoadPhrases(IPhraseSupplier<T> loader)
        {
            var wordDatas = CreateDictionary();//initialize root node
            PhraseCount = 0;
            foreach (var phrase in loader.GetPhrases())
            {
                PhraseCount++;
                var last = phrase.Phrase.Length - 1;
                var current = wordDatas;
                for (int i = 0; i < phrase.Phrase.Length; i++)
                {
                    var word = phrase.Phrase[i];
                    if (!current.TryGetValue(word, out var data))//word doesn't exist - add
                    {
                        var temp = i != last ? 
                            new PhraseWordData<T>() { Next = CreateDictionary() } : //not last in phrase - setup for next word
                            new PhraseWordData<T>() { Values = new List<T> { phrase.Value } }; //last word in phrase - add value
                        current.Add(word, temp);
                        current = temp.Next;
                    }
                    else //word exists
                    {
                        if (i != last) //not last in phrase - setup for next word
                        {
                            data.Next ??= CreateDictionary();
                            current = data.Next;
                        }
                        else //last word in phrase - add value
                        {
                            data.AddValue(phrase.Value);
                        }
                    }
                }
            }
            WordDatas = wordDatas;
        }
        IEnumerable<Phrase<T>> IPhraseParser<T>.Parse(string[] input, IPhraseValueSelector<T> selector)
        {
            for (int i = 0; i < input.Length; i++)
            {
                var temp = GetNext(WordDatas, input, i, out var data);
                var range = i..(temp + 1);
                var exists = data != null;
                yield return new Phrase<T>(
                    exists,
                    range,
                    exists ? selector.Select(data.Values) : default);
                i = temp;
            }
        }

        private static Dictionary<string, PhraseWordData<T>> CreateDictionary() => new(StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Finds next phrase starting at given startIndex
        /// </summary>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int GetNext(Dictionary<string, PhraseWordData<T>> WordDatas, string[] input, int startIndex, out PhraseWordData<T>? data)
        {
            data = null;
            PhraseWordData<T>? current = null;
            int returnIndex = startIndex;
            for (int i = startIndex; i < input.Length; i++)
            {
                if (current == null)
                {
                    if (WordDatas.TryGetValue(input[i], out current))
                    {
                        if (current.Values != null)
                        {
                            returnIndex = i;
                            data = current;
                        }
                    }
                    else
                    {
                        return returnIndex;
                    }
                }
                else
                {
                    if (current.Next != null && current.Next.TryGetValue(input[i], out current))
                    {
                        if (current.Values != null)
                        {
                            returnIndex = i;
                            data = current;
                        }
                    }
                    else
                    {
                        return returnIndex;
                    }
                }
            }
            return returnIndex;
        }

        //slower but don't know why - less IL though
        //private static int GetNext(Dictionary<string, PhraseWordData<T>> WordDatas, string[] input, int startIndex, out PhraseWordData<T>? data)
        //{
        //    data = null;
        //    int returnIndex = startIndex;
        //    if (!WordDatas.TryGetValue(input[startIndex], out var current)) return returnIndex;
        //    if (current.Values != null) data = current;
        //    for (int i = startIndex + 1; i < input.Length; i++)
        //    {
        //        if (current.Next == null || !current.Next.TryGetValue(input[i], out current)) return returnIndex;
        //        if (current.Values != null)
        //        {
        //            returnIndex = i;
        //            data = current;
        //        }
        //    }
        //    return returnIndex;
        //}
    }

    public static class PhraseParser
    {
        public static string[] Sanitize(string[] str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var input = string.Join(' ', str).Replace("_", " ");//replaces underscores with spaces
            return rgx.Replace(input, "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);//replaces all non "A-z0-9 -" with empty - also removes empties
        }
    }
}