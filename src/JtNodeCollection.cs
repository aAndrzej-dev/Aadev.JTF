using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CustomSources;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aadev.JTF
{
    [DebuggerDisplay("JtNodeCollection in {Owner.Name}, is main: {IsMainCollection}")]
    public sealed class JtNodeCollection : IJtNodeCollectionChild, IJtNodeParent, IList<IJtNodeCollectionChild>, IJtStructureCollectionElement
    {
        private readonly IJtNodeParent parent;
        private readonly JtNodeCollectionSource? @base;
        private readonly JArray? @override;
        private List<IJtNodeCollectionChild>? children;
        private List<JtNode>? nodes; // Nodes cache for main collections
        private IIdentifiersManager? childrenManager;



        [MemberNotNull(nameof(children))]
        internal List<IJtNodeCollectionChild> Children
        {
            get
            {
                if (children is null)
                {
                    BuildChildren();
                }
                return children;
            }
        }
        [MemberNotNull(nameof(children))]
        private void BuildChildren()
        {
            if (@base is null && @override is null)
            {
                children = new List<IJtNodeCollectionChild>();
            }
            else if (@base is null && @override is not null)
            {
                children = BuildChildrenNormal();
            }
            else if (@base is not null && @override is null)
            {
                children = BuildChildrenInstance();
            }
            else
            {
                children = BuildChildrenOverride();
            }
        }
        private List<IJtNodeCollectionChild> BuildChildrenOverride()
        {
            List<IJtNodeCollectionChild> list = new List<IJtNodeCollectionChild>();

            int baseIndex = -1;
            int overrideIndex = -1;

            while (true)
            {
                baseIndex++;
                overrideIndex++;
                if (@base!.Children.Count <= baseIndex)
                {
                    if (@override!.Count <= overrideIndex)
                    {
                        break;
                    }
                    else
                    {
                        list.Add(CreateChildItem(this, @override[overrideIndex]));
                    }
                }
                else if (@override!.Count <= overrideIndex)
                {
                    list.Add(@base.Children[baseIndex].CreateInstance(this, null));
                }
                else if (@override[overrideIndex]?.Type is JTokenType.Null)
                {
                    continue;
                }
                else
                {
                    list.Add(@base.Children[baseIndex].CreateInstance(this, @override[overrideIndex]));
                }
            }

            return list;
        }
        private List<IJtNodeCollectionChild> BuildChildrenInstance()
        {
            List<IJtNodeCollectionChild> list = new List<IJtNodeCollectionChild>(@base!.Children.Count);

            for (int i = 0; i < @base.Children.Count; i++)
            {
                list.Add(@base.Children[i].CreateInstance(this, null));
            }

            return list;
        }
        private List<IJtNodeCollectionChild> BuildChildrenNormal()
        {
            List<IJtNodeCollectionChild> list = new List<IJtNodeCollectionChild>(@override!.Count);

            for (int i = 0; i < @override.Count; i++)
            {
                list.Add(CreateChildItem(this, @override[i]));
            }

            return list;
        }
        private static IJtNodeCollectionChild CreateChildItem(IJtNodeParent parent, JToken source)
        {
            JtUnknownNode CreateUnknown() => new JtUnknownNode(parent);

            if (source is null)
                return CreateUnknown();
            if (source.Type is JTokenType.Array)
                return Create(parent, source);
            if (source.Type is JTokenType.String)
            {
                JtSourceReference id = (string?)source;
                return id.Type switch
                {
                    JtSourceReferenceType.External => ((IJtNodeCollectionSourceChild?)parent.SourceProvider.GetCustomSource(id))?.CreateInstance(parent, null) ?? CreateUnknown(),
                    JtSourceReferenceType.Direct => parent.SourceProvider.GetCustomSource<JtNodeSource>(id)?.CreateInstance(parent, null) ?? CreateUnknown(),
                    _ => CreateUnknown(),
                };
            }
            if (source.Type is JTokenType.Object)
            {
                JToken? @base = source["base"];
                if (@base is null)
                    return JtNode.Create(parent, source);
                IJtNodeCollectionSourceChild child = new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value;



                if (child is JtNodeCollectionSource)
                    return child.CreateInstance(parent, source["_"]);
                if (child is JtNodeSource)
                    return child.CreateInstance(parent, source);

                return CreateUnknown();
            }
            return CreateUnknown();
        }






        [Browsable(false)]
        public IJtNodeParent Parent => parent;
        [Browsable(false)] public IIdentifiersManager IdentifiersManager => Parent.GetIdentifiersManagerForChild();
        [Browsable(false)] public JtContainerNode Owner => Parent.Owner!;
        [Browsable(false)] public JTemplate Template => Parent.Template;
        [MemberNotNullWhen(true, nameof(Nodes))]
        [Browsable(false)] public bool IsMainCollection => Parent.Owner == Parent;
        [Browsable(false)] public int Count => Children.Count;
        [Browsable(false)] public bool IsReadOnly => Template.IsReadOnly;
        [Browsable(false)] public ICustomSourceProvider SourceProvider => Parent.SourceProvider;
        [Browsable(false)] public List<JtNode>? Nodes => IsMainCollection ? nodes ??= Children.SelectMany(x => x.GetNodes()).ToList() : null;

        [Browsable(false)] public bool HasExternalChildren => IsExternal || Children.Any(x => x.IsExternal);

        [Browsable(false)] public bool IsExternal => @base?.IsExternal is true;

        bool IJtStructureParentElement.HasExternalChildrenSource => IsExternal;

        IJtStructureCollectionElement IJtStructureParentElement.ChildrenCollection => this;

        bool IJtStructureParentElement.IsRoot => false;

        IJtCustomSourceDeclaration? IJtStructureInnerElement.Base => @base?.IsDeclared is true ? @base.Declaration : null;

        public IJtNodeCollectionChild this[int index] { get => Children[index]; set => Children[index] = value; }


        private JtNodeCollection(IJtNodeParent parent)
        {
            this.parent = parent;
        }
        private JtNodeCollection(IJtNodeParent parent, JArray source)
        {
            this.parent = parent;
            @override = source;
        }
        internal JtNodeCollection(IJtNodeParent parent, JtNodeCollectionSource source, JArray? @override)
        {
            this.parent = parent;
            @base = source;
            this.@override = @override;
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
                    case JtSourceReferenceType.External:
                        return parent.SourceProvider.GetCustomSource<JtNodeCollectionSource>(id)?.CreateInstance(parent, null) ?? new JtNodeCollection(parent);
                    case JtSourceReferenceType.Direct:
                    {
                        JtContainerNodeSource? element = parent.SourceProvider.GetCustomSource<JtContainerNodeSource>(id);
                        if (element is null)
                            return new JtNodeCollection(parent);
                        return element.Children.CreateInstance(parent, null);

                    }
                    default:
                        return new JtNodeCollection(parent);
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

                if (new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value is JtNodeCollectionSource ncs)
                    return new JtNodeCollection(parent, ncs, (JArray?)source["_"]);
                throw new UnreachableException();

            }
            return new JtNodeCollection(parent);
        }

        internal void BuildJson(StringBuilder sb)
        {
            if (@base is not null)
            {
                bool isAnyChildOverridden = Children.Any(x => x.IsOverridden());


                if (!isAnyChildOverridden && !@base.IsDeclared)
                    return;

                if (!isAnyChildOverridden)
                {
                    @base.BuildJson(sb);
                    return;
                }


                sb.Append('{');
                if (@base.IsDeclared)
                {
                    sb.Append("\"base\": ");
                    @base.BuildJson(sb);
                }
                if (isAnyChildOverridden)
                {
                    sb.Append(", \"_\": [");
#if NET5_0_OR_GREATER
                    Span<IJtNodeCollectionChild> listSpan = CollectionsMarshal.AsSpan(Children);
                    for (int i = 0; i < listSpan.Length; i++)
                    {
                        if (i > 0)
                            sb.Append(',');
                        IJtNodeCollectionChild item = listSpan[i];
                        if (item.IsOverridden())
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
                        if (item.IsOverridden())
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

#if NET5_0_OR_GREATER
                Span<IJtNodeCollectionChild> listSpan = CollectionsMarshal.AsSpan(Children);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    if (!isFirst)
                    {
                        sb.Append(',');
                    }
                    else
                        isFirst = false;
                    listSpan[i].BuildJson(sb);
                }
#else

                for (int i = 0; i < Children.Count; i++)
                {
                    if (!isFirst)
                    {
                        sb.Append(',');
                    }
                    else
                        isFirst = false;
                    Children[i].BuildJson(sb);
                }
#endif
                sb.Append(']');
            }
        }
        internal JtNodeCollectionSource CreateSource() => JtNodeCollectionSource.Create(this);



        public bool IsOverridden() => IsMainCollection ? Nodes.Any(x => x.IsOverridden()) : Children.Any(x => x.IsOverridden());
        public int IndexOf(IJtNodeCollectionChild item) => Children.IndexOf(item);
        public void Insert(int index, IJtNodeCollectionChild item) => Children.Insert(index, item);
        public void RemoveAt(int index) => Children.RemoveAt(index);
        public void Add(IJtNodeCollectionChild item)
        {
            Children.Add(item);

            if (item is JtNode node)
            {
                OnItemAddedToChild(node);
            }
        }
        public void Clear() => Children.Clear();
        public bool Contains(IJtNodeCollectionChild item) => Children.Contains(item);
        public void CopyTo(IJtNodeCollectionChild[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
        public bool Remove(IJtNodeCollectionChild item)
        {
            if (Children.Remove(item))
            {
                if (item is JtNode node)
                {
                    OnItemRemovedFromChild(node);
                }
                return true;
            }
            return false;
        }
        public IEnumerator<IJtNodeCollectionChild> GetEnumerator() => Children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        IEnumerable<JtNode> IJtNodeCollectionChild.GetNodes() => nodes ?? Children.SelectMany(x => x.GetNodes());
        internal void OnItemRemovedFromChild(JtNode item)
        {
            if (Parent is JtNodeCollection c)
            {
                c.OnItemRemovedFromChild(item);
            }
            else
            {
                if (!IsMainCollection)
                {
                    throw new UnreachableException();
                }
                Nodes.Remove(item);
            }
        }
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
                    throw new UnreachableException();
                }
                Nodes.Add(item);
            }
        }

        public IIdentifiersManager GetIdentifiersManagerForChild()
        {
            if (Owner.ContainerDisplayType is JtContainerType.Array)
                return new IdentifiersManager(IdentifiersManager);
            if (@base?.IsDeclared is true)
                return childrenManager ??= new IdentifiersManager(IdentifiersManager);
            return IdentifiersManager;
        }
        IJtNodeCollectionSourceChild IJtNodeCollectionChild.CreateSource() => CreateSource();
        void IJtNodeCollectionChild.BuildJson(StringBuilder sb) => BuildJson(sb);
        void IJtStructureCollectionElement.Add(IJtStructureInnerElement item) => Add((IJtNodeCollectionChild)item);
        IEnumerable<IJtStructureInnerElement> IJtStructureParentElement.GetStructureChildren() => Children;
    }
}
