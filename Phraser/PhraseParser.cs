using System.Text.RegularExpressions;

namespace Phraser
{
    public class PhraseParser<T> : IPhraseParser<T>
    {
        public Dictionary<string, PhraseWordData<T>> WordDatas { get; private set; }
        public int PhraseCount { get; private set; }
        public void LoadPhrases(IPhraseSupplier<T> loader)
        {
            var wordDatas = CreateDictionary();
            PhraseCount = 0;
            foreach (var phrase in loader.GetPhrases())
            {
                PhraseCount++;
                var last = phrase.Phrase.Length - 1;
                var current = wordDatas;
                for (int i = 0; i < phrase.Phrase.Length; i++)
                {
                    var word = phrase.Phrase[i];
                    if (current.TryGetValue(word, out var data))
                    {
                        if (i != last)
                        {
                            data.Next ??= CreateDictionary();
                        }
                        current = data.Next;
                        if (i == last)
                        {
                            data.Values ??= new List<T>();
                            data.Values.Add(phrase.Value);
                        }
                    }
                    else
                    {
                        var temp = new PhraseWordData<T>();
                        if (i == last)
                        {
                            temp.Values ??= new List<T>();
                            temp.Values.Add(phrase.Value);
                        }
                        else
                        {
                            temp.Next ??= CreateDictionary();
                        }
                        current.Add(word, temp);
                        current = temp.Next;
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

        public static string[] Sanitize(string[] str)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            var input = string.Join(' ', str).Replace("_", " ");//replaces underscores with spaces
            return rgx.Replace(input, "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);//replaces all non "A-z0-9 -" with empty - also removes empties
        }

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

        //original working code
        //private static int GetNext(Dictionary<string, PhraseWordData<T>> WordDatas, string[] input, int startIndex, out PhraseWordData<T>? data)
        //{
        //    data = null;
        //    PhraseWordData<T>? current = null;
        //    var state = WordDatas;
        //    int returnIndex = startIndex;
        //    for (int i = startIndex; i < input.Length; i++)
        //    {
        //        if (state != null && state.ContainsKey(input[i]))
        //        {
        //            current = state[input[i]];
        //            state = current.Next;
        //            if (current.Values != null)
        //            {
        //                returnIndex = i;
        //                data = current;
        //            }
        //        }
        //        else
        //        {
        //            return returnIndex;
        //        }
        //    }
        //    return returnIndex;
        //}
    }
}