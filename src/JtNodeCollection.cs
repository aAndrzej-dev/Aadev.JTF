using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;
using Aadev.JTF.CustomSources.Declarations;
using Aadev.JTF.CustomSources.Nodes;
using Aadev.JTF.Types;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF;

[DebuggerDisplay("JtNodeCollection in {Owner.Name}, is main: {IsMainCollection}")]
public sealed class JtNodeCollection : IJtInstanceStructureElement, IJtNodeParent, IList<IJtInstanceStructureElement>, IJtCommonNodeCollection
{
    private readonly IJtNodeParent parent;
    private JtNodeCollectionSource? @base;
    private readonly JArray? @override;
    private List<IJtInstanceStructureElement>? children;
    private List<JtNode>? nodes; // Nodes cache for main collections
    private IdentifiersManager? childrenManager;



    [MemberNotNull(nameof(children))]
    internal List<IJtInstanceStructureElement> Children
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
            children = new List<IJtInstanceStructureElement>();
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
    private List<IJtInstanceStructureElement> BuildChildrenOverride()
    {
        List<IJtInstanceStructureElement> list = new List<IJtInstanceStructureElement>();

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
    private List<IJtInstanceStructureElement> BuildChildrenInstance()
    {
        List<IJtInstanceStructureElement> list = new List<IJtInstanceStructureElement>(@base!.Children.Count);

        for (int i = 0; i < @base.Children.Count; i++)
        {
            list.Add(@base.Children[i].CreateInstance(this, null));
        }

        return list;
    }
    private List<IJtInstanceStructureElement> BuildChildrenNormal()
    {
        List<IJtInstanceStructureElement> list = new List<IJtInstanceStructureElement>(@override!.Count);

        for (int i = 0; i < @override.Count; i++)
        {
            list.Add(CreateChildItem(this, @override[i]));
        }

        return list;
    }
    private static IJtInstanceStructureElement CreateChildItem(IJtNodeParent parent, JToken source)
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
                JtSourceReferenceType.External => ((IJtSourceStructureElement?)parent.SourceProvider.GetCustomSource(id))?.CreateInstance(parent, null) ?? CreateUnknown(),
                JtSourceReferenceType.Direct => parent.SourceProvider.GetCustomSource<JtNodeSource>(id)?.CreateInstance(parent, null) ?? CreateUnknown(),
                _ => CreateUnknown(),
            };
        }

        if (source.Type is JTokenType.Object)
        {
            JToken? @base = source["base"];
            if (@base is null)
                return JtNode.Create(parent, source);
            IJtSourceStructureElement child = new CustomSourceBaseDeclaration(@base, parent.SourceProvider).Value;



            if (child is JtNodeCollectionSource)
                return child.CreateInstance(parent, source["_"]);
            if (child is JtNodeSource)
                return child.CreateInstance(parent, source);

            return CreateUnknown();
        }

        return CreateUnknown();
    }


    [Browsable(false)] public IJtNodeParent Parent => parent;
    [Browsable(false)] public IdentifiersManager IdentifiersManager => Parent.GetIdentifiersManagerForChild();
    [Browsable(false)] public JtContainerNode? Owner => Parent.Owner;
    [Browsable(false)] public JTemplate Template => Parent.Template;
    [MemberNotNullWhen(true, nameof(Nodes))]
    [Browsable(false)] public bool IsMainCollection => Parent.Owner == Parent || Parent == Template;
    [Browsable(false)] public int Count => Children.Count;
    [Browsable(false)] public bool IsReadOnly => Template.IsReadOnly;
    [Browsable(false)] public ICustomSourceProvider SourceProvider => Parent.SourceProvider;
    [Browsable(false)] public List<JtNode>? Nodes => IsMainCollection ? nodes ??= Children.SelectMany(x => x.GetNodes()).ToList() : null;
    [MemberNotNullWhen(true, nameof(Base))]
    [Browsable(false)] public bool IsExternal => @base?.IsExternal is true;

    public JtNodeCollectionSource? Base { get => @base; set { @base = value; nodes = null; BuildChildren(); } }

    IJtCommonParent IJtCommonContentElement.Parent => Parent;

    IJtCommonRoot IJtCommonContentElement.Root => Template;

    IJtCustomSourceDeclaration? IJtCommonContentElement.BaseDeclaration => IsExternal ? Base.Declaration : null;

    bool IJtCommonContentElement.IsRootChild => Parent is IJtCommonRoot;

    bool IJtCommonParent.HasExternalChildrenSource => IsExternal;

    public JtNodeCollection OwnersMainCollection
    {
        get
        {
            if (IsMainCollection)
                return this;
            return Parent.OwnersMainCollection;
        }
    }

    public IJtInstanceStructureElement this[int index] { get => Children[index]; set => Children[index] = value; }


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
                Span<IJtInstanceStructureElement> listSpan = CollectionsMarshal.AsSpan(Children);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    IJtInstanceStructureElement item = listSpan[i];
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
                    IJtInstanceStructureElement item = Children[i];
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
            Span<IJtInstanceStructureElement> listSpan = CollectionsMarshal.AsSpan(Children);
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
    public int IndexOf(IJtInstanceStructureElement item) => Children.IndexOf(item);
    public void Insert(int index, IJtInstanceStructureElement item) => Children.Insert(index, item);
    public void RemoveAt(int index) => Children.RemoveAt(index);
    public void Add(IJtInstanceStructureElement item)
    {
        Children.Add(item);

        if (item is JtNode node)
        {
            OnItemAddedToChild(node);
        }
    }
    public void Clear() => Children.Clear();
    public bool Contains(IJtInstanceStructureElement item) => Children.Contains(item);
    public void CopyTo(IJtInstanceStructureElement[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
    public bool Remove(IJtInstanceStructureElement item)
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
    public IEnumerator<IJtInstanceStructureElement> GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerable<JtNode> IJtInstanceStructureElement.GetNodes() => nodes ?? Children.SelectMany(x => x.GetNodes());
    private void OnItemRemovedFromChild(JtNode item)
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
    private void OnItemAddedToChild(JtNode item)
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

    public IdentifiersManager GetIdentifiersManagerForChild()
    {
        if (Parent is JTemplate)
            return IdentifiersManager;
        if (Owner!.ContainerDisplayType is JtContainerType.Array)
            return new IdentifiersManager(IdentifiersManager);
        if (@base?.IsDeclared is true)
            return childrenManager ??= new IdentifiersManager(IdentifiersManager);
        return IdentifiersManager;
    }
    IJtSourceStructureElement IJtInstanceStructureElement.CreateSource() => CreateSource();

    void IJtJsonBuildable.BuildJson(StringBuilder sb) => BuildJson(sb);

    IEnumerable<IJtCommonContentElement> IJtCommonParent.EnumerateChildrenElements() => Children;
    IJtCommonNodeCollection IJtCommonParent.GetChildrenElementsCollection() => this;

    IJtCommonContentElement IList<IJtCommonContentElement>.this[int index]
    {
        get => this[index]; set
        {
            if (value is IJtInstanceStructureElement element)
            {
                this[index] = element;
            }
            else
                throw new Exception();
        }
    }
    int IList<IJtCommonContentElement>.IndexOf(IJtCommonContentElement item)
    {
        if (item is IJtInstanceStructureElement element)
        {
            return IndexOf(element);
        }
        else
            throw new Exception();
    }
    void IList<IJtCommonContentElement>.Insert(int index, IJtCommonContentElement item)
    {
        if (item is IJtInstanceStructureElement element)
        {
            Insert(index, element);
        }
        else
            throw new Exception();
    }
    void ICollection<IJtCommonContentElement>.Add(IJtCommonContentElement item)
    {
        if (item is IJtInstanceStructureElement element)
        {
            Add(element);
        }
        else
            throw new Exception();
    }
    bool ICollection<IJtCommonContentElement>.Contains(IJtCommonContentElement item)
    {
        if (item is IJtInstanceStructureElement element)
        {
            return Contains(element);
        }
        else
            throw new Exception();
    }
    void ICollection<IJtCommonContentElement>.CopyTo(IJtCommonContentElement[] array, int arrayIndex)
    {
        if (array is IJtInstanceStructureElement[] element)
        {
            CopyTo(element, arrayIndex);
        }
        else
            throw new Exception();
    }
    bool ICollection<IJtCommonContentElement>.Remove(IJtCommonContentElement item)
    {
        if (item is IJtInstanceStructureElement element)
        {
            return Remove(element);
        }
        else
            throw new Exception();
    }
    IEnumerator<IJtCommonContentElement> IEnumerable<IJtCommonContentElement>.GetEnumerator() => Children.GetEnumerator();
}
