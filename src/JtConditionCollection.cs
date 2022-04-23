using System;
using System.Collections;
using System.Collections.Generic;

namespace Aadev.JTF
{
    public class JtConditionCollection : IList<JtCondition>, IReadOnlyList<JtCondition>, IList, IEquatable<JtConditionCollection>
    {
        private readonly List<JtCondition> jtConditions = new List<JtCondition>();

        /// <inheritdoc/>
        public int Count => ((ICollection<JtCondition>)jtConditions).Count;
        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<JtCondition>)jtConditions).IsReadOnly;
        /// <inheritdoc/>
        public bool IsFixedSize => ((IList)jtConditions).IsFixedSize;
        /// <inheritdoc/>
        public bool IsSynchronized => ((ICollection)jtConditions).IsSynchronized;
        /// <inheritdoc/>
        public object SyncRoot => ((ICollection)jtConditions).SyncRoot;

        object? IList.this[int index] { get => ((IList)jtConditions)[index]; set => ((IList)jtConditions)[index] = value; }
        /// <inheritdoc/>
        public JtCondition this[int index] { get => ((IList<JtCondition>)jtConditions)[index]; set => ((IList<JtCondition>)jtConditions)[index] = value; }

        public bool Check(CheckOperation checkOperation, string? value)
        {
            if (checkOperation is CheckOperation.Or)
            {
                foreach (JtCondition item in jtConditions)
                {
                    if (item.Check(value))
                        return true;
                }

            }
            else
            {
                foreach (JtCondition item in jtConditions)
                {
                    if (!item.Check(value))
                        return false;
                }

            }

            return false;
        }
        public enum CheckOperation
        {
            And,
            Or
        }


        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as JtConditionCollection);
        /// <inheritdoc/>
        public bool Equals(JtConditionCollection? other)
        {
            if (other is null || other.Count != Count) return false;


            foreach (JtCondition item in jtConditions)
            {
                if (!other.Contains(item))
                    return false;

            }
            return true;
        }
        /// <inheritdoc/>
        public override int GetHashCode() => 935463109 + EqualityComparer<List<JtCondition>>.Default.GetHashCode(jtConditions);
        /// <inheritdoc/>
        public int IndexOf(JtCondition item) => ((IList<JtCondition>)jtConditions).IndexOf(item);
        /// <inheritdoc/>
        public void Insert(int index, JtCondition item) => ((IList<JtCondition>)jtConditions).Insert(index, item);
        /// <inheritdoc/>
        public void RemoveAt(int index) => ((IList<JtCondition>)jtConditions).RemoveAt(index);
        /// <inheritdoc/>
        public void Add(JtCondition item) => ((ICollection<JtCondition>)jtConditions).Add(item);
        /// <inheritdoc/>
        public void Clear() => ((ICollection<JtCondition>)jtConditions).Clear();
        /// <inheritdoc/>
        public bool Contains(JtCondition item) => ((ICollection<JtCondition>)jtConditions).Contains(item);
        /// <inheritdoc/>
        public void CopyTo(JtCondition[] array, int arrayIndex) => ((ICollection<JtCondition>)jtConditions).CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(JtCondition item) => ((ICollection<JtCondition>)jtConditions).Remove(item);
        /// <inheritdoc/>
        public IEnumerator<JtCondition> GetEnumerator() => ((IEnumerable<JtCondition>)jtConditions).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)jtConditions).GetEnumerator();
        /// <inheritdoc/>
        public int Add(object? value) => ((IList)jtConditions).Add(value);
        /// <inheritdoc/>
        public bool Contains(object? value) => ((IList)jtConditions).Contains(value);
        /// <inheritdoc/>
        public int IndexOf(object? value) => ((IList)jtConditions).IndexOf(value);
        /// <inheritdoc/>
        public void Insert(int index, object? value) => ((IList)jtConditions).Insert(index, value);
        /// <inheritdoc/>
        public void Remove(object? value) => ((IList)jtConditions).Remove(value);
        /// <inheritdoc/>
        public void CopyTo(Array array, int index) => ((ICollection)jtConditions).CopyTo(array, index);

        public static bool operator ==(JtConditionCollection left, JtConditionCollection right) => EqualityComparer<JtConditionCollection>.Default.Equals(left, right);
        public static bool operator !=(JtConditionCollection left, JtConditionCollection right) => !(left == right);
    }
}