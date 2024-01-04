using System.Collections.Frozen;

namespace Phraser
{
    /// <summary>
    /// Frozen Phrase Parser - Consumes more memory until words are hit and then converted to immutable datatypes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FrozenPhraseParser<T> : IPhraseParser<T>
    {
        public FrozenDictionary<string, FrozenPhraseWordData<T>> WordDatas { get; private set; }

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
                            new FrozenPhraseWordData<T>() { OriginalNext = CreateDictionary() } : //not last in phrase - setup for next word
                            new FrozenPhraseWordData<T>() { OriginalValues = [phrase.Value] }; //last word in phrase - add value
                        current.Add(word, temp);
                        current = temp.OriginalNext;
                    }
                    else //word exists
                    {
                        if (i != last) //not last in phrase - setup for next word
                        {
                            data.OriginalNext ??= CreateDictionary();
                            current = data.OriginalNext;
                        }
                        else //last word in phrase - add value
                        {
                            data.AddValue(phrase.Value);
                        }
                    }
                }
            }
            WordDatas = wordDatas.ToFrozenDictionary<string, FrozenPhraseWordData<T>>();
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
                    exists ? selector.Select(data.Values.Value) : default);
                i = temp;
            }
        }

        private static Dictionary<string, FrozenPhraseWordData<T>> CreateDictionary() => new(StringComparer.OrdinalIgnoreCase);


        /// <summary>
        /// Finds next phrase starting at given startIndex
        /// </summary>
        /// <param name="WordDatas"></param>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int GetNext(FrozenDictionary<string, FrozenPhraseWordData<T>> WordDatas, string[] input, int startIndex, out FrozenPhraseWordData<T>? data)
        {
            data = null;
            int returnIndex = startIndex;
            if (!WordDatas.TryGetValue(input[startIndex], out var current)) return returnIndex;
            if (current.Values.Value != null) data = current;
            for (int i = startIndex + 1; i < input.Length; i++)
            {
                if (current.Next.Value == null || !current.Next.Value.TryGetValue(input[i], out current)) return returnIndex;
                if (current.Values.Value != null)
                {
                    returnIndex = i;
                    data = current;
                }
            }
            return returnIndex;
        }

    }
}