using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aadev.JTF
{
    [Serializable]
    public class JtNodeCollection : IList<JtNode>, IReadOnlyList<JtNode>, IJtCollection, IList
    {
        private List<JtNode>? tokens;
        private readonly IJtParentNode owner;
        private string? customSourceId;

        public string? CustomSourceId
        {
            get => customSourceId; set
            {
                customSourceId = value;

                if (customSourceId == value)
                    return;
                customSourceId = value;
                tokens ??= new List<JtNode>();

                ReadOnly = false;
                if (string.IsNullOrWhiteSpace(value))
                {
                    customSourceId = null;
                    return;
                }
                if (customSourceId is null)
                {
                    return;
                }
                Clear();

                if (customSourceId.StartsWith('@'))
                {
                    AddRange((JtNode[])owner.Template.GetCustomValue(customSourceId.AsSpan(1).ToString())!.Value);
                }
                else if (customSourceId.StartsWith('#'))
                {
                    JtNode? node = owner.IdentifiersManager.GetNodeById(customSourceId.AsSpan(1).ToString());
                    if (node is JtBlock block)
                    {

                        AddRange(block.Children.ToArray());
                    }
                    else
                    {
                        throw new Exception();
                    }
                }

                ReadOnly = true;
            }
        }
        public List<JtNode> Tokens
        {
            get
            {
                if (tokens is null)
                {
                    tokens = new List<JtNode>();
                    if (CustomSourceId.StartsWith('@'))
                    {
                        tokens.AddRange((JtNode[])owner.Template.GetCustomValue(CustomSourceId.AsSpan(1).ToString())!.Value);
                    }
                    else if (CustomSourceId.StartsWith('#'))
                    {
                        JtNode? node = owner.IdentifiersManager.GetNodeById(CustomSourceId.AsSpan(1).ToString());
                        if (node is JtBlock block)
                        {

                            tokens.AddRange(block.Children.ToArray());

                        }
                        else if (node is null)
                        {
                            JtNode? tNode = owner.Template.GetNodeById(CustomSourceId.AsSpan(1).ToString());
                            if (tNode is JtBlock tblock)
                            {
                                tokens.AddRange(tblock.Children.ToArray());
                            }
                            else
                            {
                                throw new Exception();

                            }
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    ReadOnly = true;

                }

                return tokens;
            }
        }

        public int Count => Tokens.Count;

        bool ICollection<JtNode>.IsReadOnly => ((IList<JtNode>)Tokens).IsReadOnly;

        public JtNode this[int index]
        {
            get => Tokens[index];
            set
            {
                if (ReadOnly)
                    return;
                Tokens[index].Parent = null;

                value.Parent = owner;
                Tokens[index] = value;
            }
        }

        internal JtNodeCollection(IJtParentNode owner)
        {
            tokens = new List<JtNode>();
            this.owner = owner;
        }
        internal JtNodeCollection(IJtParentNode owner, JToken? source)
        {
            this.owner = owner;
            if (source is JArray)
            {
                tokens = new List<JtNode>();
                foreach (JObject item in source)
                {
                    Add(JtNode.Create(item, owner.Template, owner is JArray ? new BlankIdentifiersManager() : owner.IdentifiersManager));
                }
            }
            else if (((JValue?)source)?.Value is string str)
            {
                if (!str.StartsWith("@") && !str.StartsWith("#"))
                    throw new Exception("Custom values name must starts with '@' or '#'");

                customSourceId = str;
            }
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

        bool IList.IsFixedSize => ((IList)Tokens).IsFixedSize;

        bool IList.IsReadOnly => ((IList)Tokens).IsReadOnly;

        bool ICollection.IsSynchronized => ((IList)Tokens).IsSynchronized;

        object ICollection.SyncRoot => ((IList)Tokens).SyncRoot;

        object? IList.this[int index] { get => ((IList)Tokens)[index]; set => ((IList)Tokens)[index] = value; }

        public int IndexOf(JtNode item) => Tokens.IndexOf(item);
        public void Insert(int index, JtNode item)
        {
            if (ReadOnly)
                return;

            if (ContainsSimilarToken(item))
                return;


            item.Parent = owner;
            Tokens.Insert(index, item);
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

            Tokens.Add(item);
            item.Parent = owner;

        }
        public void Clear()
        {
            if (ReadOnly)
                return;
            for (int i = 0; i < Tokens.Count; i++)
            {
                Tokens[i].Parent = null;
            }
            Tokens.Clear();
        }
        public bool Contains(JtNode item) => Tokens.Contains(item);
        void ICollection<JtNode>.CopyTo(JtNode[] array, int arrayIndex) => Tokens.CopyTo(array, arrayIndex);
        public bool Remove(JtNode item)
        {
            if (ReadOnly)
                return false;
            if (item is null)
                return false;
            if (!Tokens.Contains(item))
                return false;
            Tokens.Remove(item);
            item.Parent = null;

            return true;
        }
        public IEnumerator<JtNode> GetEnumerator() => Tokens.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


        private bool ContainsSimilarToken(JtNode token)
        {
            for (int i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].Name == token.Name && Tokens[i].Type == token.Type && Tokens[i].Condition == token.Condition)
                    return true;
            }
            return false;
        }

        int IList.Add(object? value) => ((IList)Tokens).Add(value);
        bool IList.Contains(object? value) => ((IList)Tokens).Contains(value);
        int IList.IndexOf(object? value) => ((IList)Tokens).IndexOf(value);
        void IList.Insert(int index, object? value) => ((IList)Tokens).Insert(index, value);
        void IList.Remove(object? value) => ((IList)Tokens).Remove(value);
        void ICollection.CopyTo(Array array, int index) => ((IList)Tokens).CopyTo(array, index);
        public void BuildJson(StringBuilder sb)
        {
            if (CustomSourceId is null)
            {
                sb.Append('[');

                for (int i = 0; i < Count; i++)
                {
                    if (i != 0)
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