using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Aadev.JTF.CollectionBuilders;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources.Declarations;
using Aadev.JTF.Tools;
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


    IJtCommonParent? IJtCommonContentElement.Parent => (IJtCommonParent?)Parent;

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

    internal override void BuildJsonDeclaration(JsonBuilder jb)
    {
        if (@base is not null)
        {
            bool isAnyChildOverridden = Children.Any(x => x.IsOverridden());


            if (!isAnyChildOverridden && !@base.IsDeclared)
                return;

            if (!isAnyChildOverridden)
            {
                jb.AddValue(@base);
                return;
            }


            jb.StartBlock();
            if (@base.IsDeclared)
            {
                jb.AddProperty("base", @base);
            }

            if (isAnyChildOverridden)
            {
                jb.AddProperty("_");
                jb.StartArray();
#if NET5_0_OR_GREATER
                Span<IJtSourceStructureElement> listSpan = CollectionsMarshal.AsSpan(Children);
                for (int i = 0; i < listSpan.Length; i++)
                {
                    IJtSourceStructureElement item = listSpan[i];
                    if (item.IsOverridden())
                        jb.AddValue(item);
                    else
                        jb.AddValue("{}");
                }
#else
                for (int i = 0; i < Children.Count; i++)
                {
                    IJtSourceStructureElement item = Children[i];
                    if (item.IsOverridden())
                        jb.AddValue(item);
                    else
                    {
                        jb.AddValue("{}");
                    }
                }
#endif
                jb.EndArray();
            }

            jb.EndBlock();
        }
        else
        {
            jb.StartArray();

#if NET5_0_OR_GREATER
            Span<IJtSourceStructureElement> listSpan = CollectionsMarshal.AsSpan(Children);
            for (int i = 0; i < listSpan.Length; i++)
            {
                jb.AddValue(listSpan[i]);
            }
#else
            for (int i = 0; i < Children.Count; i++)
            {
                jb.AddValue(Children[i]);
            }
#endif
            jb.EndArray();
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
            if (value is not IJtSourceStructureElement element)
                throw new InvalidCastException($"Cannot cast '{nameof(value)}' from '{value?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement)}");

            this[index] = element;
        }
    }
    int IList<IJtCommonContentElement>.IndexOf(IJtCommonContentElement item)
    {
        if (item is not IJtSourceStructureElement element)
            throw new InvalidCastException($"Cannot cast '{nameof(item)}' from '{item?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement)}");

        return IndexOf(element);
    }
    void IList<IJtCommonContentElement>.Insert(int index, IJtCommonContentElement item)
    {
        if (item is not IJtSourceStructureElement element)
            throw new InvalidCastException($"Cannot cast '{nameof(item)}' from '{item?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement)}");

        Insert(index, element);
    }
    void ICollection<IJtCommonContentElement>.Add(IJtCommonContentElement item)
    {
        if (item is not IJtSourceStructureElement element)
            throw new InvalidCastException($"Cannot cast '{nameof(item)}' from '{item?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement)}");

        Add(element);
    }
    bool ICollection<IJtCommonContentElement>.Contains(IJtCommonContentElement item)
    {
        if (item is not IJtSourceStructureElement element)
            throw new InvalidCastException($"Cannot cast '{nameof(item)}' from '{item?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement)}");

        return Contains(element);
    }
    void ICollection<IJtCommonContentElement>.CopyTo(IJtCommonContentElement[] array, int arrayIndex)
    {
        if (array is not IJtSourceStructureElement[] element)
            throw new InvalidCastException($"Cannot cast '{nameof(array)}' from '{array?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement[])}");

        CopyTo(element, arrayIndex);
    }
    bool ICollection<IJtCommonContentElement>.Remove(IJtCommonContentElement item)
    {
        if (item is not IJtSourceStructureElement element)
            throw new InvalidCastException($"Cannot cast '{nameof(item)}' from '{item?.GetType()?.ToString() ?? "null"}' to {typeof(IJtSourceStructureElement)}");

        return Remove(element);
    }
    IEnumerator<IJtCommonContentElement> IEnumerable<IJtCommonContentElement>.GetEnumerator() => Children.GetEnumerator();
}
