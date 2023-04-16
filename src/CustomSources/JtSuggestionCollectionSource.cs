using Aadev.JTF.CollectionBuilders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtSuggestionCollectionSource<TSuggestion> : CustomSource, IJtSuggestionCollectionSource, IJtSuggestionCollectionSourceChild<TSuggestion>, IList<IJtSuggestionCollectionSourceChild<TSuggestion>>
    {
        private JtSourceReference id;
        private IJtSuggestionCollectionSourceBuilder<TSuggestion>? suggestionsBuilder;
        private List<IJtSuggestionCollectionSourceChild<TSuggestion>>? suggestions;





        [MemberNotNull(nameof(suggestions))]
        internal List<IJtSuggestionCollectionSourceChild<TSuggestion>> Suggestions
        {
            get
            {
                if (suggestions is null)
                {
                    if (suggestionsBuilder is null)
                        suggestions = new List<IJtSuggestionCollectionSourceChild<TSuggestion>>();
                    else
                    {
                        suggestions = suggestionsBuilder.Build(this);
                        suggestionsBuilder = null;
                    }
                }
                return suggestions;
            }
        }

        public bool IsSavable => !Id.IsEmpty || Suggestions.Count > 0;

        public Type SuggestionType => typeof(TSuggestion);

        public override bool IsExternal => false;

        public int Count => Suggestions.Count;

        public bool IsReadOnly => false;

        internal JtSourceReference Id
        {
            get => id;
            set
            {
                if (value.IsEmpty)
                {
                    id = JtSourceReference.Empty;
                    return;
                }
                if (value.Type is not JtSourceReferenceType.Dynamic)
                    throw new JtfException("Cannot set non-dynamic id to a suggestion collection. Use custom sources instead.");
                id = value;
            }
        }

        public IJtSuggestionCollectionSourceChild<TSuggestion> this[int index] { get => Suggestions[index]; set => Suggestions[index] = value; }

        private JtSuggestionCollectionSource(IJtCustomSourceParent parent) : base(parent) { }
        private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JArray? source) : base(parent)
        {
            if (source is not null)
                suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollectionSource<TSuggestion>(source);
        }
        private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JtSourceReference id) : base(parent)
        {
            Id = id;
        }

        private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JtSuggestionCollection<TSuggestion> source) : base(parent)
        {
            if (source is not null)
            {
                Id = source.Id;
                suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollectionSource<TSuggestion>(source);
            }
        }

        internal override void BuildJsonDeclaration(StringBuilder sb)
        {
            if (!Id.IsEmpty)
            {
                sb.Append($"\"{Id}\"");
            }
            else
            {
                sb.Append('[');
#if NET5_0_OR_GREATER
                Span<IJtSuggestionCollectionSourceChild<TSuggestion>> listSpan = CollectionsMarshal.AsSpan(Suggestions);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    listSpan[i].BuildJson(sb);

                }
#else
                for (int i = 0; i < Suggestions.Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');

                    Suggestions[i].BuildJson(sb);

                }
#endif
                sb.Append(']');
            }
        }

        public static JtSuggestionCollectionSource<TSuggestion> Create(IJtCustomSourceParent parent) => new JtSuggestionCollectionSource<TSuggestion>(parent);

        /// <summary>
        /// Creates new <see cref="JtSuggestionCollectionSource"/> from JSON value. Returns null if value is null or cannot create collection.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Exception is thrown when <paramref name="parent"/> is <see cref="null"/></exception>
        public static JtSuggestionCollectionSource<TSuggestion>? TryCreate(IJtCustomSourceParent parent, JToken? value)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));

            if (value?.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)value;
                return id.Type switch
                {
                    JtSourceReferenceType.Local or JtSourceReferenceType.Direct or JtSourceReferenceType.Dynamic => new JtSuggestionCollectionSource<TSuggestion>(parent, id),
                    JtSourceReferenceType.External => parent.SourceProvider.GetCustomSource<JtSuggestionCollectionSource<TSuggestion>>(id),
                    _ => null,
                };
            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollectionSource<TSuggestion>(parent, (JArray)value);
            }
            return null;
        }

        public static JtSuggestionCollectionSource<TSuggestion> Create(IJtCustomSourceParent parent, JToken? value)
        {
            return TryCreate(parent, value) ?? new JtSuggestionCollectionSource<TSuggestion>(parent);
        }
        internal static JtSuggestionCollectionSource<TSuggestion> Create(IJtCustomSourceParent parent, JtSuggestionCollection<TSuggestion> source) => new JtSuggestionCollectionSource<TSuggestion>(parent, source);
        void IJtSuggestionCollectionSource.BuildJson(StringBuilder sb) => BuildJson(sb);
        internal JtSuggestionCollection<TSuggestion> CreateInstance()
        {
            if (id.IsEmpty)
                return JtSuggestionCollection<TSuggestion>.Create(this);
            else
                return JtSuggestionCollection<TSuggestion>.Create(id); // For dynamic declaration only
        }
        IJtSuggestionCollection IJtSuggestionCollectionSource.CreateInstance() => CreateInstance();
        IJtSuggestionCollectionChild<TSuggestion> IJtSuggestionCollectionSourceChild<TSuggestion>.CreateInstance() => CreateInstance();
        void IJtSuggestionCollectionSourceChild<TSuggestion>.BuildJson(StringBuilder sb) => BuildJson(sb);
        public int IndexOf(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.IndexOf(item);
        public void Insert(int index, IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Insert(index, item);
        public void RemoveAt(int index) => Suggestions.RemoveAt(index);
        public void Add(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Add(item);
        public void Clear() => Suggestions.Clear();
        public bool Contains(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Contains(item);
        public void CopyTo(IJtSuggestionCollectionSourceChild<TSuggestion>[] array, int arrayIndex) => Suggestions.CopyTo(array, arrayIndex);
        public bool Remove(IJtSuggestionCollectionSourceChild<TSuggestion> item) => Suggestions.Remove(item);
        public IEnumerator<IJtSuggestionCollectionSourceChild<TSuggestion>> GetEnumerator() => Suggestions.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Suggestions.GetEnumerator();
    }
}
