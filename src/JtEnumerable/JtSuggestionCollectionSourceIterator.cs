using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtSuggestionCollectionSourceIterator<T> : JtIterator<IJtSuggestionCollectionSourceChild<T>>
    {
        private readonly JtSuggestionCollectionSource<T> parent;
        private readonly JArray source;
        private int index = -1;

        public JtSuggestionCollectionSourceIterator(JtSuggestionCollectionSource<T> parent, JArray source)
        {
            this.parent = parent;
            this.source = source;
        }
        private IJtSuggestionCollectionSourceChild<T> CreateSuggestionItem(JToken source)
        {
            if (source is null)
                return new JtSuggestionSource<T>(parent, default!, "Unknown");
            if (source.Type is JTokenType.Array || source.Type is JTokenType.String)
            {
                return JtSuggestionCollectionSource<T>.Create(parent, source);
            }
            if (source.Type is JTokenType.Object)
            {
                return new JtSuggestionSource<T>(parent, (JObject)source);
            }
            return new JtSuggestionSource<T>(parent, default!, "Unknown");
        }
        public override JtIterator<IJtSuggestionCollectionSourceChild<T>> Clone() => new JtSuggestionCollectionSourceIterator<T>(parent, source);
        public override bool MoveNext()
        {
            index++;
            if (index >= source.Count)
            {
                Current = null!;
                return false;
            }
            Current = CreateSuggestionItem(source[index]);
            return true;
        }
    }
}
