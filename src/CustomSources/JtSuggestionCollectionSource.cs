using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtSuggestionCollectionSource<T> : CustomSource, IJtSuggestionCollectionSource, IJtSuggestionCollectionSourceChild<T>
    {
        private readonly JtSourceReference id;
        internal readonly IJtEnumerable<IJtSuggestionCollectionSourceChild<T>> suggestionEnumerable;

        public bool IsSaveable => !id.IsEmpty || suggestionEnumerable.Enumerate().Count > 0;

        public Type SuggestionType => typeof(T);

        private JtSuggestionCollectionSource(ICustomSourceParent parent) : base(parent)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
        }
        private JtSuggestionCollectionSource(ICustomSourceParent parent, JArray? source) : base(parent)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollectionSource<T>(this, source);
        }
        private JtSuggestionCollectionSource(ICustomSourceParent parent, JtSourceReference id) : base(parent)
        {
            suggestionEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
            this.id = id;
        }

        private JtSuggestionCollectionSource(ICustomSourceParent parent, JtSuggestionCollection<T> source) : base(parent)
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
                suggestionEnumerable = JtEnumerable.JtEnumerable.CreateJtSuggestionCollectionSource<T>(parent, source);
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
                List<IJtSuggestionCollectionSourceChild<T>> list = suggestionEnumerable.Enumerate();
#if NET5_0_OR_GREATER
                Span<IJtSuggestionCollectionSourceChild<T>> listSpan = CollectionsMarshal.AsSpan(list);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    listSpan[i].BuildJson(sb);

                }
#else
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    list[i].BuildJson(sb);

                }
#endif
                sb.Append(']');
            }
        }

        public static JtSuggestionCollectionSource<T> Create(ICustomSourceParent parent) => new JtSuggestionCollectionSource<T>(parent);
        public static JtSuggestionCollectionSource<T> Create(ICustomSourceParent parent, JToken? value)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            if (value?.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)value;
                switch (id.Type)
                {
                    case JtSourceReferenceType.None:
                    default:
                        return new JtSuggestionCollectionSource<T>(parent);
                    case JtSourceReferenceType.Local:
                    case JtSourceReferenceType.Direct:
                    case JtSourceReferenceType.Dynamic:
                        return new JtSuggestionCollectionSource<T>(parent, id);
                    case JtSourceReferenceType.External:
                        return parent.SourceProvider.GetCustomSource<JtSuggestionCollectionSource<T>>(id) ?? new JtSuggestionCollectionSource<T>(parent, id);
                }
            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollectionSource<T>(parent, (JArray)value);
            }
            return new JtSuggestionCollectionSource<T>(parent);
        }
        internal static JtSuggestionCollectionSource<T> Create(ICustomSourceParent parent, JtSuggestionCollection<T> source) => new JtSuggestionCollectionSource<T>(parent, source);
        void IJtSuggestionCollectionSource.BuildJson(StringBuilder sb) => BuildJson(sb);
        internal JtSuggestionCollection<T> CreateInstance() => JtSuggestionCollection<T>.Create(this, id);
        IJtSuggestionCollection IJtSuggestionCollectionSource.CreateInstance() => CreateInstance();
        IJtSuggestionCollectionChild<T> IJtSuggestionCollectionSourceChild<T>.CreateInstance() => CreateInstance();
        void IJtSuggestionCollectionSourceChild<T>.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
