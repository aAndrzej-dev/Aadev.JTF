using Aadev.JTF.CollectionBuilders;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class JtSuggestionCollectionSource<T> : CustomSource, IJtSuggestionCollectionSource, IJtSuggestionCollectionSourceChild<T>, IList<IJtSuggestionCollectionSourceChild<T>>
    {
        private JtSourceReference id;

        


        private IJtCollectionBuilder<IJtSuggestionCollectionSourceChild<T>>? suggestionsBuilder;
        private List<IJtSuggestionCollectionSourceChild<T>>? suggestions;

        [MemberNotNull(nameof(suggestions))]
        internal List<IJtSuggestionCollectionSourceChild<T>> Suggestions
        {
            get
            {
                if (suggestions is null)
                {
                    suggestions = suggestionsBuilder!.Build();
                    suggestionsBuilder = null;
                }
                return suggestions;
            }
        }

        public bool IsSavable => !Id.IsEmpty || Suggestions.Count > 0;

        public Type SuggestionType => typeof(T);

        public override bool IsExternal => false;

        public int Count => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).Count;

        public bool IsReadOnly => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).IsReadOnly;

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

        public IJtSuggestionCollectionSourceChild<T> this[int index] { get => ((IList<IJtSuggestionCollectionSourceChild<T>>)Suggestions)[index]; set => ((IList<IJtSuggestionCollectionSourceChild<T>>)Suggestions)[index] = value; }

        private JtSuggestionCollectionSource(IJtCustomSourceParent parent) : base(parent)
        {
            suggestionsBuilder = JtCollectionBuilder.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
        }
        private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JArray? source) : base(parent)
        {
            suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollectionSource<T>(this, source);
        }
        private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JtSourceReference id) : base(parent)
        {
            Id = id;
            suggestionsBuilder = JtCollectionBuilder.CreateEmpty<IJtSuggestionCollectionSourceChild<T>>();
        }

        private JtSuggestionCollectionSource(IJtCustomSourceParent parent, JtSuggestionCollection<T> source) : base(parent)
        {
            Id = source.Id;
            suggestionsBuilder = JtCollectionBuilder.CreateJtSuggestionCollectionSource<T>(parent, source);
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
                Span<IJtSuggestionCollectionSourceChild<T>> listSpan = CollectionsMarshal.AsSpan(Suggestions);
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

        public static JtSuggestionCollectionSource<T> Create(IJtCustomSourceParent parent) => new JtSuggestionCollectionSource<T>(parent);
        public static JtSuggestionCollectionSource<T> Create(IJtCustomSourceParent parent, JToken? value)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            if (value?.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)value;
                return id.Type switch
                {
                    JtSourceReferenceType.Local or JtSourceReferenceType.Direct or JtSourceReferenceType.Dynamic => new JtSuggestionCollectionSource<T>(parent, id),
                    JtSourceReferenceType.External => parent.SourceProvider.GetCustomSource<JtSuggestionCollectionSource<T>>(id) ?? new JtSuggestionCollectionSource<T>(parent, id),
                    _ => new JtSuggestionCollectionSource<T>(parent),
                };
            }
            if (value?.Type is JTokenType.Array)
            {
                return new JtSuggestionCollectionSource<T>(parent, (JArray)value);
            }
            return new JtSuggestionCollectionSource<T>(parent);
        }
        internal static JtSuggestionCollectionSource<T> Create(IJtCustomSourceParent parent, JtSuggestionCollection<T> source) => new JtSuggestionCollectionSource<T>(parent, source);
        void IJtSuggestionCollectionSource.BuildJson(StringBuilder sb) => BuildJson(sb);
        internal JtSuggestionCollection<T> CreateInstance()
        {
            if (id.IsEmpty)
                return JtSuggestionCollection<T>.Create(this);
            else
                return JtSuggestionCollection<T>.Create(id); // For dynamic declaration only
        }
        IJtSuggestionCollection IJtSuggestionCollectionSource.CreateInstance() => CreateInstance();
        IJtSuggestionCollectionChild<T> IJtSuggestionCollectionSourceChild<T>.CreateInstance() => CreateInstance();
        void IJtSuggestionCollectionSourceChild<T>.BuildJson(StringBuilder sb) => BuildJson(sb);
        public int IndexOf(IJtSuggestionCollectionSourceChild<T> item) => ((IList<IJtSuggestionCollectionSourceChild<T>>)Suggestions).IndexOf(item);
        public void Insert(int index, IJtSuggestionCollectionSourceChild<T> item) => ((IList<IJtSuggestionCollectionSourceChild<T>>)Suggestions).Insert(index, item);
        public void RemoveAt(int index) => ((IList<IJtSuggestionCollectionSourceChild<T>>)Suggestions).RemoveAt(index);
        public void Add(IJtSuggestionCollectionSourceChild<T> item) => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).Add(item);
        public void Clear() => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).Clear();
        public bool Contains(IJtSuggestionCollectionSourceChild<T> item) => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).Contains(item);
        public void CopyTo(IJtSuggestionCollectionSourceChild<T>[] array, int arrayIndex) => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).CopyTo(array, arrayIndex);
        public bool Remove(IJtSuggestionCollectionSourceChild<T> item) => ((ICollection<IJtSuggestionCollectionSourceChild<T>>)Suggestions).Remove(item);
        public IEnumerator<IJtSuggestionCollectionSourceChild<T>> GetEnumerator() => ((IEnumerable<IJtSuggestionCollectionSourceChild<T>>)Suggestions).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Suggestions).GetEnumerator();
    }
}
