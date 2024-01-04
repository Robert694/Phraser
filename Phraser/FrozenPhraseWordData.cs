using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Phraser
{
    public class FrozenPhraseWordData<T>
    {
        public FrozenPhraseWordData()
        {
            Next = new Lazy<FrozenDictionary<string, FrozenPhraseWordData<T>>>(GetFrozenNext);
            Values = new Lazy<ImmutableList<T>>(GetFrozenValues);
        }
        public Lazy<FrozenDictionary<string, FrozenPhraseWordData<T>>> Next { get; set; }
        public Lazy<ImmutableList<T>> Values { get; set; }

        public Dictionary<string, FrozenPhraseWordData<T>> OriginalNext { get; set; }
        public List<T> OriginalValues { get; set; }

        public void AddValue(T value)
        {
            OriginalValues ??= [];
            OriginalValues.Add(value);
        }

        public FrozenDictionary<string, FrozenPhraseWordData<T>> GetFrozenNext()
        {
            if (OriginalNext == null) return null;
            var value = OriginalNext.ToFrozenDictionary<string, FrozenPhraseWordData<T>>();
            OriginalNext.Clear();
            OriginalNext = null;
            return value;
        }

        public ImmutableList<T> GetFrozenValues()
        {
            if (OriginalValues == null) return null;
            var value = OriginalValues.ToImmutableList<T>();
            OriginalValues.Clear();
            OriginalValues = null;
            return value;
        }
    }
}