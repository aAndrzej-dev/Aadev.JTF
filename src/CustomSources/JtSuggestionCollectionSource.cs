using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtSuggestionCollectionSource<T> : CustomSource, IJtSuggestionCollectionSource, IJtSuggestionCollectionSourceChild<T>
    {
        private readonly JtCustomResourceIdentifier id;
        internal readonly IJtEnumerable<IJtSuggestionCollectionSourceChild<T>> suggestionEnumerable;

        public bool IsSaveable => !id.IsEmpty || suggestionEnumerable.Enumerate().Count > 0;

        private JtSuggestionCollectionSource(ICustomSourceParent parent, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
        }
        private JtSuggestionCollectionSource(ICustomSourceParent parent, JArray? source, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollectionSource<T>(this, source, sourceProvider);
        }
        private JtSuggestionCollectionSource(ICustomSourceParent parent, JtCustomResourceIdentifier id, ICustomSourceProvider sourceProvider) : base(parent, sourceProvider)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            this.id = id;
        }

        public JtSuggestionCollectionSource(ICustomSourceParent parent, JtSuggestionCollection<T> source) : base(parent, null)
        {
            if (source is null)
            {
                suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            }
            else if (!source.Id.IsEmpty)
            {
                id = source.Id;
                suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            }
            else
                suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollectionSource<T>(source, parent);
        }

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            if (!id.IsEmpty)
            {
                sb.Append($"\"{id}\"");
            }
            else
            {
                sb.Append('[');
                System.Collections.Generic.List<IJtSuggestionCollectionSourceChild<T>> list = suggestionEnumerable.Enumerate();
                for (int i = 0; i < list.Count; i++)
                {
                    IJtSuggestionCollectionSourceChild<T>? item = list[i];
                    if (i > 0)
                        sb.Append(',');

                    item.BuildJson(sb);

                }
                sb.Append(']');
            }
        }


        public static JtSuggestionCollectionSource<T> Create(ICustomSourceParent parent, JToken? value, ICustomSourceProvider sourceProvider)
        {
            if (value?.Type is JTokenType.String)
            {
                JtCustomResourceIdentifier id = (string?)value;
                if (id.Type is JtCustomResourceIdentifierType.None)
                    return new JtSuggestionCollectionSource<T>(parent, sourceProvider);
                if (id.Type is JtCustomResourceIdentifierType.Dynamic)
                    return new JtSuggestionCollectionSource<T>(parent, id, sourceProvider);
                if (id.Type is JtCustomResourceIdentifierType.External)
                    return sourceProvider.GetCustomSource<JtSuggestionCollectionSource<T>>(id) ?? new JtSuggestionCollectionSource<T>(parent, id, sourceProvider);
            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollectionSource<T>(parent, (JArray)value, sourceProvider);
            }
            return new JtSuggestionCollectionSource<T>(parent, sourceProvider);
        }
        internal static JtSuggestionCollectionSource<T> Create(JtSuggestionCollection<T> source, ICustomSourceParent parent) => new JtSuggestionCollectionSource<T>(parent, source);
        void IJtSuggestionCollectionSource.BuildJson(StringBuilder sb) => BuildJson(sb);
        internal JtSuggestionCollection<T> CreateInstance() => JtSuggestionCollection<T>.Create(this, id);
        internal JtSuggestionCollection<T> CreateInstance(JtCustomResourceIdentifier id) => JtSuggestionCollection<T>.Create(this, id);
        IJtSuggestionCollection IJtSuggestionCollectionSource.CreateInstance() => CreateInstance();
        IJtSuggestionCollectionChild<T> IJtSuggestionCollectionSourceChild<T>.CreateInstance() => CreateInstance();
        IJtSuggestionCollection IJtSuggestionCollectionSource.CreateInstance(JtCustomResourceIdentifier id) => CreateInstance(id);
        void IJtSuggestionCollectionSourceChild<T>.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
