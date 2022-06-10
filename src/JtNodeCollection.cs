using System;
using System.Collections;
using System.Collections.Generic;

namespace Aadev.JTF
{
    [Serializable]
    public class JtNodeCollection : IList<JtNode>, IReadOnlyList<JtNode>
    {
        private readonly List<JtNode> tokens;
        private readonly IJtParentNode owner;


        public int Count => tokens.Count;

        bool ICollection<JtNode>.IsReadOnly => ((IList<JtNode>)tokens).IsReadOnly;

        public JtNode this[int index]
        {
            get => tokens[index];
            set
            {
                if (ReadOnly)
                    return;
                tokens[index].Parent = null;

                value.Parent = owner;
                tokens[index] = value;
            }
        }

        internal JtNodeCollection(IJtParentNode owner)
        {
            tokens = new List<JtNode>();
            this.owner = owner;
        }

        public void AddRange(JtNode[] items)
        {
            if (ReadOnly)
                return;
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }

        public bool ReadOnly { get; set; }

        public int IndexOf(JtNode item) => tokens.IndexOf(item);
        public void Insert(int index, JtNode item)
        {
            if (ReadOnly)
                return;

            if (ContainsSimilarToken(item))
                return;


            item.Parent = owner;
            tokens.Insert(index, item);
        }
        public void RemoveAt(int index)
        {
            if (ReadOnly)
                return;
            Remove(this[index]);
        }

        public void Add(JtNode item)
        {
            if (ReadOnly)
                return;
            if (item is null)
                return;

            if (ContainsSimilarToken(item))
                throw new Exception($"Cannot add multiple tokens with the same name, type and conditions.\nName: {item.Name}\nType: {item.Type.DisplayName}");

            tokens.Add(item);
            item.Parent = owner;

        }
        public void Clear()
        {
            if (ReadOnly)
                return;
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i].Parent = null;
            }
            tokens.Clear();
        }
        public bool Contains(JtNode item) => tokens.Contains(item);
        void ICollection<JtNode>.CopyTo(JtNode[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove(JtNode item)
        {
            if (ReadOnly)
                return false;
            if (item is null)
                return false;
            if (!tokens.Contains(item))
                return false;
            tokens.Remove(item);
            item.Parent = null;

            return true;
        }
        public IEnumerator<JtNode> GetEnumerator() => tokens.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private bool ContainsSimilarToken(JtNode token)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Name == token.Name && tokens[i].Type == token.Type && tokens[i].Conditions == token.Conditions)
                    return true;
            }
            return false;
        }
    }

}