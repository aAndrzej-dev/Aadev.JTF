﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace Aadev.JTF
{
    [Serializable]
    public class TokensCollection : IList<JtToken>, IReadOnlyList<JtToken>
    {
        private readonly List<JtToken> tokens;
        private readonly IJtParentType owner;

        public event EventHandler? OnTokenAdded;

        public int Count => tokens.Count;

        public bool IsReadOnly => ((IList<JtToken>)tokens).IsReadOnly;

        public JtToken this[int index]
        {
            get => tokens[index]; set
            {
                tokens[index].Parent = null;

                value.Parent = owner;
                tokens[index] = value;
            }
        }

        internal TokensCollection(IJtParentType owner)
        {
            tokens = new List<JtToken>();
            this.owner = owner;
        }

        public void AddRange(JtToken[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Add(items[i]);
            }
        }



        public int IndexOf(JtToken item) => tokens.IndexOf(item);
        public void Insert(int index, JtToken item)
        {


            if (ContainsToken(item))
                return;


            item.Parent = owner;
            tokens.Insert(index, item);
        }
        public void RemoveAt(int index) => Remove(this[index]);
        public void Add(JtToken item)
        {
            if (item is null)
                return;

            if (ContainsToken(item))
                throw new Exception($"Cannot add multiple tokens with the same name, type and conditions.\nName: {item.Name}\nType: {item.Type.DisplayName}");

            OnTokenAdded?.Invoke(this, EventArgs.Empty);

            tokens.Add(item);
            item.Parent = owner;

        }
        public void Clear()
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i].Parent = null;
            }
            tokens.Clear();
        }
        public bool Contains(JtToken item) => tokens.Contains(item);
        void ICollection<JtToken>.CopyTo(JtToken[] array, int arrayIndex) => throw new NotImplementedException();
        public bool Remove(JtToken item)
        {
            if (item is null)
                return false;
            if (!tokens.Contains(item))
                return false;
            tokens.Remove(item);
            item.Parent = null;

            return true;
        }
        public IEnumerator<JtToken> GetEnumerator() => tokens.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private bool ContainsToken(JtToken token)
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