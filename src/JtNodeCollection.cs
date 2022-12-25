using Aadev.JTF.CustomSources;
using Aadev.JTF.JtEnumerable;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF
{
    public sealed class JtNodeCollection : IJtNodeCollectionChild, IJtNodeParent, IList<IJtNodeCollectionChild>
    {
        private readonly JtSourceReference id;
        internal readonly IJtEnumerable<IJtNodeCollectionChild> nodeEnumerable;
        private readonly JtNodeCollectionSource? @base;
        private List<JtNode>? nodes;
        private List<IJtNodeCollectionChild>? children;
        private IIdentifiersManager? childrenManager;

        public IJtNodeParent Parent { get; }
        public IIdentifiersManager IdentifiersManager => Parent.IdentifiersManager;

        public JtContainer Owner => Parent.Owner!;


        public JTemplate Template => Parent.Template;
        public bool IsMainCollection => Parent.Owner == Parent;

        public int Count => Children.Count;

        public bool IsReadOnly => false;

        public ICustomSourceProvider SourceProvider => Parent.SourceProvider;
        public IJtNodeCollectionChild this[int index] { get => Children[index]; set => Children[index] = value; }


        private JtNodeCollection(IJtNodeParent parent)
        {
            Parent = parent;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtNodeCollectionChild>();
        }

        internal JtNodeCollection(IJtNodeParent parent, JtSourceReference id)
        {
            Parent = parent;
            this.id = id;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreateEmpty<IJtNodeCollectionChild>();
        }
        private JtNodeCollection(IJtNodeParent parent, JArray source)
        {
            Parent = parent;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreatJtNodeCollection(this, source);
        }
        internal JtNodeCollection(IJtNodeParent parent, JtNodeCollectionSource source, JArray? @override)
        {
            Parent = parent;
            @base = source;
            nodeEnumerable = JtEnumerable.JtEnumerable.CreatJtNodeCollection(this, source, @override);
        }

        public static JtNodeCollection Create(IJtNodeParent parent) => new JtNodeCollection(parent);
        public static JtNodeCollection Create(IJtNodeParent parent, JToken? source)
        {
            if (parent is null)
                throw new ArgumentNullException(nameof(parent));
            if (source?.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)source;
                switch (id.Type)
                {
                    case JtSourceReferenceType.None:
                    default:
                        return new JtNodeCollection(parent);
                    case JtSourceReferenceType.Local:
                    case JtSourceReferenceType.Dynamic:
                        return new JtNodeCollection(parent, id);
                    case JtSourceReferenceType.External:
                        return parent.SourceProvider.GetCustomSource<JtNodeCollectionSource>(id)?.CreateInstance(parent, null) ?? new JtNodeCollection(parent, id);
                    case JtSourceReferenceType.Direct:
                    {
                        JtContainerNodeSource? element = parent.SourceProvider.GetCustomSource<JtContainerNodeSource>(id);
                        if (element is null)
                            return new JtNodeCollection(parent, id);
                        return element.Children.CreateInstance(parent, null);

                    }
                }
            }
            if (source?.Type is JTokenType.Array)
            {
                return new JtNodeCollection(parent, (JArray)source);
            }
            if (source?.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                    return new JtNodeCollection(parent);
                
                if(new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value is JtNodeCollectionSource ncs)
                    return new JtNodeCollection(parent, ncs, (JArray?)source["_"]);
#if NET7_0_OR_GREATER
                throw new UnreachableException();
#else
                throw new InternalException();
#endif

            }
            return new JtNodeCollection(parent);
        }

        internal void BuildJson(StringBuilder sb)
        {
            if (@base != null)
            {
                bool isAnyChildOverriden = Children.Any(x => x.IsOverriden());


                if (!isAnyChildOverriden && !@base.IsDeclarated)
                    return;

                if(!isAnyChildOverriden)
                {
                    @base.BuildJson(sb);
                    return;
                }    


                sb.Append('{');
                if (@base.IsDeclarated)
                {
                    sb.Append("\"base\": ");
                    @base.BuildJson(sb);
                }
                if (isAnyChildOverriden)
                {
                    sb.Append(", \"_\": [");
#if NET5_0_OR_GREATER
                    Span<IJtNodeCollectionChild> listSpan = CollectionsMarshal.AsSpan(Children);
                    for (int i = 0; i < listSpan.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        IJtNodeCollectionChild item = listSpan[i];
                        if (item.IsOverriden())
                            item.BuildJson(sb);
                        else
                        {
                            sb.Append("{}");
                        }
                    }
#else
                    for (int i = 0; i < Children.Count; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        IJtNodeCollectionChild item = Children[i];
                        if (item.IsOverriden())
                            item.BuildJson(sb);
                        else
                        {
                            sb.Append("{}");
                        }
                    }
#endif
                    sb.Append(']');
                }
                sb.Append('}');
            }
            else
            {
                sb.Append('[');
                bool isFirst = true;
                foreach (IJtNodeCollectionChild item in nodeEnumerable)
                {
                    if (!isFirst)
                    {
                        sb.Append(',');
                    }
                    else
                        isFirst = false;
                    item.BuildJson(sb);
                }
                sb.Append(']');
            }
        }
        internal JtNodeCollectionSource CreateSource() => JtNodeCollectionSource.Create(this);

        public List<JtNode>? Nodes => IsMainCollection ? nodes ??= nodeEnumerable.SelectMany(x => x.GetNodes()).ToList() : null;
        private List<IJtNodeCollectionChild> Children => children ??= nodeEnumerable.Enumerate();

        public bool HasExternalChildren => (@base != null && @base.IsDeclarated) || Children.Any(x => x is JtNodeCollection n && n.HasExternalChildren);

        public bool IsExternal => @base?.IsDeclarated ?? !id.IsEmpty;


        public bool IsOverriden() => IsMainCollection ? Nodes!.Any(x => x.IsOverriden()) : nodeEnumerable.Enumerate().Any(x => x.IsOverriden());
        public int IndexOf(IJtNodeCollectionChild item) => Children.IndexOf(item);
        public void Insert(int index, IJtNodeCollectionChild item) => Children.Insert(index, item);
        public void RemoveAt(int index) => Children.RemoveAt(index);
        public void Add(IJtNodeCollectionChild item)
        {
            Children.Add(item);
        
            if(item is JtNode n && Parent is JtNodeCollection c)
            {
                c.OnItemAddedToChild(n);
            }
        }
        public void Clear() => Children.Clear();
        public bool Contains(IJtNodeCollectionChild item) => Children.Contains(item);
        public void CopyTo(IJtNodeCollectionChild[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        public bool Remove(IJtNodeCollectionChild item) => Children.Remove(item);
        public IEnumerator<IJtNodeCollectionChild> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerable<JtNode> IJtNodeCollectionChild.GetNodes() => nodes ?? nodeEnumerable.SelectMany(x => x.GetNodes());


        internal void OnItemAddedToChild(JtNode item)
        {
            if (Parent is JtNodeCollection c)
            {
                c.OnItemAddedToChild(item);
            }
            else
            {
                if (!IsMainCollection)
                {
#if NET7_0_OR_GREATER
                    throw new UnreachableException();
#else
                    throw new InternalException();
#endif
                }
                nodes ??= nodeEnumerable.SelectMany(x => x.GetNodes()).ToList();
                nodes.Add(item);
            }
        }

        public IIdentifiersManager GetIdentifiersManagerForChild()
        {
            if (Owner.ContainerDisplayType is JtContainerType.Array)
                return new IdentifiersManager(IdentifiersManager);
            if ((@base is null || !@base.IsDeclarated)&& id.IsEmpty)
                return IdentifiersManager;
            return childrenManager ??= new IdentifiersManager(IdentifiersManager);
        }

        IJtNodeCollectionSourceChild IJtNodeCollectionChild.CreateSource() => CreateSource();
        void IJtNodeCollectionChild.BuildJson(StringBuilder sb) => BuildJson(sb);
    }
}
