using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Aadev.JTF.CollectionBuilders;
using Aadev.JTF.Common;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources.Declarations;

public sealed class CustomSourceBaseDeclaration : IJtCustomSourceDeclaration
{
    private IdentifiersManager? identifiersManager;
    private readonly ICustomSourceProvider sourceProvider;

    internal CustomSourceBaseDeclaration(JToken source, ICustomSourceProvider sourceProvider)
    {
        this.sourceProvider = sourceProvider;
        Value = JtNodeSourceCollectionBuilder.CreateChildItem(this, source);
    }

    public IJtSourceStructureElement Value { get; }
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsDeclaringSource => Value is not null;
    public string Name => "Inline Base Declaration";

    public ICustomSourceProvider SourceProvider => sourceProvider;

    IJtCustomSourceDeclaration IJtCustomSourceParent.Declaration => this;


    [MemberNotNull(nameof(identifiersManager))]
    public IdentifiersManager IdentifiersManager => identifiersManager ??= new IdentifiersManager(null);


    JtNodeSource? IJtNodeSourceParent.Owner => null;
    bool IJtCommonParent.HasExternalChildrenSource => false;
    CustomSource? IJtCustomSourceDeclaration.Value => (CustomSource)Value;

    public CustomSourceType Type
    {
        get
        {
            if (Value is JtNodeCollectionSource)
                return CustomSourceType.NodeCollection;
            if (Value is JtNodeSource)
                return CustomSourceType.Node;
            throw new IndexOutOfRangeException();
        }
    }

    void IJtJsonBuildable.BuildJson(StringBuilder sb) => ((CustomSource)Value).BuildJsonDeclaration(sb);

    public override string ToString() => Name;
    IEnumerable<IJtCommonContentElement> IJtCommonParent.EnumerateChildrenElements()
    {
        yield return Value;
    }
    IJtCommonNodeCollection IJtCommonParent.GetChildrenElementsCollection() => throw new NotSupportedException();
    IJtCommonNode IJtCommonRoot.CreateNodeElement(IJtCommonParent parent, JtNodeType type) => type.CreateEmptySource((IJtNodeSourceParent)parent);
    IJtCommonNodeCollection IJtCommonRoot.CreateCollectionElement(IJtCommonParent parent) => JtNodeCollectionSource.Create((IJtNodeSourceParent)parent);
}
