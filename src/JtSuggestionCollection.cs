using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    /// <summary>
    /// Collection of <see cref="JtSuggestion{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of <see cref="JtSuggestion{T}"/></typeparam>
    public class JtSuggestionCollection<T> : IList<JtSuggestion<T>>, IReadOnlyList<JtSuggestion<T>>, IJtSuggestionCollection
    {
        private readonly List<JtSuggestion<T>> suggestions;
        private readonly JtNode owner;
        private string? customSourceId;

        /// <summary>
        /// Custom source for items of collection
        /// </summary>
        public string? CustomSourceId
        {
            get => customSourceId;
            set
            {
                if (customSourceId == value || value is null || value.Length <= 1)
                    return;
                customSourceId = value;
                Clear();
                if (customSourceId.StartsWith("@"))
                    AddRange((JtSuggestion<T>[])owner.Template.GetCustomValue(customSourceId.AsSpan(1).ToString())!.Value);
            }
        }

        /// <summary>
        /// Creates empty collection of <see cref="JtSuggestion{T}"/>
        /// </summary>
        /// <param name="owner">Owner node</param>
        public JtSuggestionCollection(JtNode owner)
        {
            suggestions = new List<JtSuggestion<T>>();
            this.owner = owner;
        }
        /// <summary>
        /// Load collection for <see cref="JToken"/>
        /// </summary>
        /// <param name="owner">Owner node</param>
        /// <param name="source">Source of collection</param>
        /// <exception cref="Exception"></exception>
        public JtSuggestionCollection(JtNode owner, JToken? source)
        {
            suggestions = new List<JtSuggestion<T>>();
            this.owner = owner;

            if (source is JArray values)
            {
                foreach (JObject item in values)
                {
                    Add(new JtSuggestion<T>(item));
                }
            }
            else if (((JValue?)source)?.Value is string suggestionsStr)
            {
                if (suggestionsStr.StartsWith("@") || suggestionsStr.StartsWith("$"))
                {
                    CustomSourceId = suggestionsStr;
                }
                else
                    throw new Exception("Custom values name must starts with '@' or '$'");
            }
        }


        public JtSuggestion<T> this[int index] { get => ((IList<JtSuggestion<T>>)suggestions)[index]; set => ((IList<JtSuggestion<T>>)suggestions)[index] = value; }

        public int Count => ((ICollection<JtSuggestion<T>>)suggestions).Count;

        public bool IsReadOnly => ((ICollection<JtSuggestion<T>>)suggestions).IsReadOnly;

        public bool IsFixedSize => ((IList)suggestions).IsFixedSize;

        public bool IsSynchronized => ((ICollection)suggestions).IsSynchronized;

        public object SyncRoot => ((ICollection)suggestions).SyncRoot;

        public Type ValueType => typeof(T);

        IJtSuggestion IList<IJtSuggestion>.this[int index] { get => suggestions[index]; set => suggestions[index] = (JtSuggestion<T>)value; }
        object? IList.this[int index] { get => ((IList)suggestions)[index]; set => ((IList)suggestions)[index] = value; }

        public void Add(JtSuggestion<T> item) => ((ICollection<JtSuggestion<T>>)suggestions).Add(item);
        public void AddRange(JtSuggestion<T>[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }
        public void Clear() => ((ICollection<JtSuggestion<T>>)suggestions).Clear();
        public bool Contains(JtSuggestion<T> item) => ((ICollection<JtSuggestion<T>>)suggestions).Contains(item);
        public void CopyTo(JtSuggestion<T>[] array, int arrayIndex) => ((ICollection<JtSuggestion<T>>)suggestions).CopyTo(array, arrayIndex);
        public IEnumerator<JtSuggestion<T>> GetEnumerator() => ((IEnumerable<JtSuggestion<T>>)suggestions).GetEnumerator();
        public int IndexOf(JtSuggestion<T> item) => ((IList<JtSuggestion<T>>)suggestions).IndexOf(item);
        public void Insert(int index, JtSuggestion<T> item) => ((IList<JtSuggestion<T>>)suggestions).Insert(index, item);
        public bool Remove(JtSuggestion<T> item) => ((ICollection<JtSuggestion<T>>)suggestions).Remove(item);
        public void RemoveAt(int index) => ((IList<JtSuggestion<T>>)suggestions).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)suggestions).GetEnumerator();
        public int Add(object? value) => ((IList)suggestions).Add(value);
        public bool Contains(object? value) => ((IList)suggestions).Contains(value);
        public int IndexOf(object? value) => ((IList)suggestions).IndexOf(value);
        public void Insert(int index, object? value) => ((IList)suggestions).Insert(index, value);
        public void Remove(object? value) => ((IList)suggestions).Remove(value);
        public void CopyTo(Array array, int index) => ((ICollection)suggestions).CopyTo(array, index);
        public int IndexOf(IJtSuggestion item) => suggestions.IndexOf((JtSuggestion<T>)item);
        public void Insert(int index, IJtSuggestion item) => suggestions.Insert(index, (JtSuggestion<T>)item);
        public void Add(IJtSuggestion item) => suggestions.Add((JtSuggestion<T>)item);
        public bool Contains(IJtSuggestion item) => suggestions.Contains((JtSuggestion<T>)item);
        public void CopyTo(IJtSuggestion[] array, int arrayIndex) => suggestions.CopyTo((JtSuggestion<T>[])array, arrayIndex);
        public bool Remove(IJtSuggestion item) => suggestions.Remove((JtSuggestion<T>)item);
        IEnumerator<IJtSuggestion> IEnumerable<IJtSuggestion>.GetEnumerator() => suggestions.GetEnumerator();
        public void BuildJson(StringBuilder sb)
        {
            if (CustomSourceId is null)
            {
                sb.Append('[');
                for (int i = 0; i < Count; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    this[i].BulidJson(sb);
                }
                sb.Append(']');
            }
            else
            {
                sb.Append($"\"{CustomSourceId}\"");
            }
        }
    }
}
