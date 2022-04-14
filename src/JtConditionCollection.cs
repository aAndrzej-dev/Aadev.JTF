using System;
using System.Collections;
using System.Collections.Generic;

namespace Aadev.JTF
{
    public class JtConditionCollection : IList<JtCondition>, IReadOnlyList<JtCondition>, IList
    {
        private readonly List<JtCondition> jtConditions = new List<JtCondition>();

        public int Count => ((ICollection<JtCondition>)jtConditions).Count;

        public bool IsReadOnly => ((ICollection<JtCondition>)jtConditions).IsReadOnly;

        public bool IsFixedSize => ((IList)jtConditions).IsFixedSize;

        public bool IsSynchronized => ((ICollection)jtConditions).IsSynchronized;

        public object SyncRoot => ((ICollection)jtConditions).SyncRoot;

        object? IList.this[int index] { get => ((IList)jtConditions)[index]; set => ((IList)jtConditions)[index] = value; }
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



        public override bool Equals(object? obj) => Equals(obj as JtConditionCollection);
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
        public override int GetHashCode() => 935463109 + EqualityComparer<List<JtCondition>>.Default.GetHashCode(jtConditions);
        public int IndexOf(JtCondition item) => ((IList<JtCondition>)jtConditions).IndexOf(item);
        public void Insert(int index, JtCondition item) => ((IList<JtCondition>)jtConditions).Insert(index, item);
        public void RemoveAt(int index) => ((IList<JtCondition>)jtConditions).RemoveAt(index);
        public void Add(JtCondition item) => ((ICollection<JtCondition>)jtConditions).Add(item);
        public void Clear() => ((ICollection<JtCondition>)jtConditions).Clear();
        public bool Contains(JtCondition item) => ((ICollection<JtCondition>)jtConditions).Contains(item);
        public void CopyTo(JtCondition[] array, int arrayIndex) => ((ICollection<JtCondition>)jtConditions).CopyTo(array, arrayIndex);
        public bool Remove(JtCondition item) => ((ICollection<JtCondition>)jtConditions).Remove(item);
        public IEnumerator<JtCondition> GetEnumerator() => ((IEnumerable<JtCondition>)jtConditions).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)jtConditions).GetEnumerator();
        public int Add(object? value) => ((IList)jtConditions).Add(value);
        public bool Contains(object? value) => ((IList)jtConditions).Contains(value);
        public int IndexOf(object? value) => ((IList)jtConditions).IndexOf(value);
        public void Insert(int index, object? value) => ((IList)jtConditions).Insert(index, value);
        public void Remove(object? value) => ((IList)jtConditions).Remove(value);
        public void CopyTo(Array array, int index) => ((ICollection)jtConditions).CopyTo(array, index);

        public static bool operator ==(JtConditionCollection left, JtConditionCollection right) => EqualityComparer<JtConditionCollection>.Default.Equals(left, right);
        public static bool operator !=(JtConditionCollection left, JtConditionCollection right) => !(left == right);
    }
}