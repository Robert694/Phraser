using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Phraser
{
    public class UnsafePhraseParser<T> : IPhraseParser<T>
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
                    ref var data = ref CollectionsMarshal.GetValueRefOrAddDefault(current, word, out var exists);
                    if (!exists)
                    {
                        var temp = i != last ?
                            new PhraseWordData<T>() { Next = CreateDictionary() } : //not last in phrase - setup for next word
                            new PhraseWordData<T>() { Values = new List<T> { phrase.Value } }; //last word in phrase - add value
                        data = temp;
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
        /// <param name="WordDatas"></param>
        /// <param name="input"></param>
        /// <param name="startIndex"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private static int GetNext(Dictionary<string, PhraseWordData<T>> WordDatas, string[] input, int startIndex, out PhraseWordData<T>? data)
        {
            data = null;
            int returnIndex = startIndex;
            ref var current = ref CollectionsMarshal.GetValueRefOrNullRef(WordDatas, input[startIndex]);
            if (Unsafe.IsNullRef(ref current)) return returnIndex;
            if (current.Values != null) data = current;
            for (int i = startIndex + 1; i < input.Length; i++)
            {
                if (current.Next == null) return returnIndex;
                current = ref CollectionsMarshal.GetValueRefOrNullRef(current.Next, input[i]);
                if (Unsafe.IsNullRef(ref current)) return returnIndex;

                if (current.Values != null)
                {
                    returnIndex = i;
                    data = current;
                }
            }
            return returnIndex;
        }
    }
}