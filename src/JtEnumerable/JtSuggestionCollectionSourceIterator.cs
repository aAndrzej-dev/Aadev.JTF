using Aadev.JTF.CustomSources;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtSuggestionCollectionSourceIterator<T> : JtIterator<IJtSuggestionCollectionSourceChild<T>>
    {
        private readonly JtSuggestionCollectionSource<T> parent;
        private readonly JArray source;
        private readonly ICustomSourceProvider sourceProvider;
        private int index = -1;

        public JtSuggestionCollectionSourceIterator(JtSuggestionCollectionSource<T> parent, JArray source, ICustomSourceProvider sourceProvider)
        {
            this.parent = parent;
            this.source = source;
            this.sourceProvider = sourceProvider;
        }
        private IJtSuggestionCollectionSourceChild<T> CreateSuggestionItem(JToken source)
        {
            if (source is null)
                return new JtSuggestionSource<T>(parent, default!, "Unknown", sourceProvider);
            if (source.Type is JTokenType.Array || source.Type is JTokenType.String)
            {
                return JtSuggestionCollectionSource<T>.Create(parent, source, sourceProvider);
            }
            if (source.Type is JTokenType.Object)
            {
                return new JtSuggestionSource<T>(parent, (JObject)source, sourceProvider);
            }
            return new JtSuggestionSource<T>(parent, default!, "Unknown", sourceProvider);
        }
        public override JtIterator<IJtSuggestionCollectionSourceChild<T>> Clone() => new JtSuggestionCollectionSourceIterator<T>(parent, source, sourceProvider);
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
