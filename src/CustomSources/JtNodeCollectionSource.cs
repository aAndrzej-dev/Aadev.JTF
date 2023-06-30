using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Aadev.JTF.CollectionBuilders;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources.Declarations;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;

public sealed class JtNodeCollectionSource : CustomSource, IJtSourceStructureElement, IJtNodeSourceParent, IList<IJtSourceStructureElement>, IJtCommonNodeCollection
{
    private IJtNodeCollectionSourceBuilder? childrenBuilder;
    private readonly JtNodeCollectionSource? @base;
    private List<IJtSourceStructureElement>? children;

    [MemberNotNull(nameof(children))]
    internal List<IJtSourceStructureElement> Children
    {
        get
        {
            if (children is null)
            {
                if (childrenBuilder is null)
                {
                    children = new List<IJtSourceStructureElement>();
                }
                else
                {
                    children = childrenBuilder.Build(this);
                    childrenBuilder = null;
                }
            }

            return children;
        }
    }


    public override bool IsExternal => @base?.IsDeclared ?? IsDeclared;
    public override IJtCustomSourceDeclaration? BaseDeclaration => base.BaseDeclaration ?? @base?.Declaration;

    public bool HasExternalChildrenSource => IsExternal;


    public JtNodeSource? Owner => ((IJtNodeSourceParent?)Parent)?.Owner;


    public int Count => Children.Count;
    bool ICollection<IJtSourceStructureElement>.IsReadOnly => false;


    IJtCommonParent IJtCommonContentElement.Parent => (IJtCommonParent)Parent;

    IJtCommonRoot IJtCommonContentElement.Root => Declaration;
    bool IJtCommonContentElement.IsRootChild => IsDeclared;

    bool ICollection<IJtCommonContentElement>.IsReadOnly => false;


    public IJtSourceStructureElement this[int index] { get => Children[index]; set => Children[index] = value; }

    private JtNodeCollectionSource(IJtNodeSourceParent parent) : base(parent) { }

    private JtNodeCollectionSource(JtNodeCollection instance) : base(new CustomSourceFormInstanceDeclaration(instance.Owner, null))
    {
        childrenBuilder = JtCollectionBuilder.CreateJtNodeSourceCollection(instance);
    }

    private JtNodeCollectionSource(IJtNodeSourceParent parent, JtNodeCollectionSource @base, JArray? @override) : base(parent)
    {
        childrenBuilder = JtCollectionBuilder.CreateJtNodeSourceCollection(@base, @override);
        this.@base = @base;
    }

    private JtNodeCollectionSource(IJtNodeSourceParent parent, JArray? jArray) : base(parent)
    {
        childrenBuilder = JtCollectionBuilder.CreateJtNodeSourceCollection(jArray);
    }

    internal override void BuildJsonDeclaration(StringBuilder sb)
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
                Span<IJtSourceStructureElement> listSpan = CollectionsMarshal.AsSpan(Children);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    if (i > 0)
                        sb.Append(',');
                    IJtSourceStructureElement item = listSpan[i];
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
                    IJtSourceStructureElement item = Children[i];
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
            Span<IJtSourceStructureElement> listSpan = CollectionsMarshal.AsSpan(Children);
            for (int i = 0; i < listSpan.Length; i++)
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                listSpan[i].BuildJson(sb);
            }
#else
            for (int i = 0; i < Children.Count; i++)
            {
                if (!isFirst)
                    sb.Append(',');
                else
                    isFirst = false;
                Children[i].BuildJson(sb);
            }
#endif
            sb.Append(']');
        }
    }
    internal static JtNodeCollectionSource Create(IJtNodeSourceParent parent) => new JtNodeCollectionSource(parent);
    internal static JtNodeCollectionSource Create(IJtNodeSourceParent parent, JToken? source)
    {
        if (source?.Type is JTokenType.String)
        {
            return parent.SourceProvider.GetCustomSource<JtNodeCollectionSource>((string?)source) ?? new JtNodeCollectionSource(parent);
        }

        return new JtNodeCollectionSource(parent, (JArray?)source);

    }

    internal static JtNodeCollectionSource Create(JtNodeCollection instance) => new JtNodeCollectionSource(instance);
    internal JtNodeCollectionSource CreateOverride(IJtNodeSourceParent parent, JArray? @override) => new JtNodeCollectionSource(parent, this, @override);
    IJtInstanceStructureElement IJtSourceStructureElement.CreateInstance(IJtNodeParent parent, JToken? @override) => CreateInstance(parent, @override);
    public JtNodeCollection CreateInstance(IJtNodeParent parent, JToken? @override) => new JtNodeCollection(parent, this, @override as JArray);
    IJtSourceStructureElement IJtSourceStructureElement.CreateOverride(IJtNodeSourceParent parent, JToken? @override) => CreateOverride(parent, (JArray?)@override);
    void IJtJsonBuildable.BuildJson(StringBuilder sb) => BuildJson(sb);

    public int IndexOf(IJtSourceStructureElement item) => Children.IndexOf(item);
    public void Insert(int index, IJtSourceStructureElement item) => Children.Insert(index, item);
    public void RemoveAt(int index) => Children.RemoveAt(index);
    public void Add(IJtSourceStructureElement item) => Children.Add(item);
    public void Clear() => Children.Clear();
    public bool Contains(IJtSourceStructureElement item) => Children.Contains(item);
    public void CopyTo(IJtSourceStructureElement[] array, int arrayIndex) => Children.CopyTo(array, arrayIndex);
    public bool Remove(IJtSourceStructureElement item) => Children.Remove(item);
    public IEnumerator<IJtSourceStructureElement> GetEnumerator() => Children.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Children.GetEnumerator();
    public bool IsOverridden() => Children.Any(x => x.IsOverridden());
    IEnumerable<IJtCommonContentElement> IJtCommonParent.EnumerateChildrenElements() => Children;
    IJtCommonNodeCollection IJtCommonParent.GetChildrenElementsCollection() => this;


    IJtCommonContentElement IList<IJtCommonContentElement>.this[int index]
    {
        get => this[index]; set
        {
            if (value is IJtSourceStructureElement element)
            {
                this[index] = element;
            }
            else
                throw new Exception();
        }
    }
    int IList<IJtCommonContentElement>.IndexOf(IJtCommonContentElement item)
    {
        if (item is IJtSourceStructureElement element)
        {
            return IndexOf(element);
        }
        else
            throw new Exception();
    }
    void IList<IJtCommonContentElement>.Insert(int index, IJtCommonContentElement item)
    {
        if (item is IJtSourceStructureElement element)
        {
            Insert(index, element);
        }
        else
            throw new Exception();
    }
    void ICollection<IJtCommonContentElement>.Add(IJtCommonContentElement item)
    {
        if (item is IJtSourceStructureElement element)
        {
            Add(element);
        }
        else
            throw new Exception();
    }
    bool ICollection<IJtCommonContentElement>.Contains(IJtCommonContentElement item)
    {
        if (item is IJtSourceStructureElement element)
        {
            return Contains(element);
        }
        else
            throw new Exception();
    }
    void ICollection<IJtCommonContentElement>.CopyTo(IJtCommonContentElement[] array, int arrayIndex)
    {
        if (array is IJtSourceStructureElement[] element)
        {
            CopyTo(element, arrayIndex);
        }
        else
            throw new Exception();
    }
    bool ICollection<IJtCommonContentElement>.Remove(IJtCommonContentElement item)
    {
        if (item is IJtSourceStructureElement element)
        {
            return Remove(element);
        }
        else
            throw new Exception();
    }
    IEnumerator<IJtCommonContentElement> IEnumerable<IJtCommonContentElement>.GetEnumerator() => Children.GetEnumerator();
}
