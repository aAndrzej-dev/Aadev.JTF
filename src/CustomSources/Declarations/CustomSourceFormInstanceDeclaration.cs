using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Aadev.JTF.Common;

namespace Aadev.JTF.CustomSources.Declarations;

public sealed class CustomSourceFormInstanceDeclaration : IJtCustomSourceDeclaration
{
    private IdentifiersManager? identifiersManager;

    internal CustomSourceFormInstanceDeclaration(JtNode instance, IJtSourceStructureElement value)
    {
        Instance = instance;
        Value = value;
    }

    public JtNode Instance { get; }
    public IJtSourceStructureElement Value { get; }

    public bool IsDeclaringSource => Value is not null;
    public ICustomSourceProvider SourceProvider => Instance;
    public string Name => $"#{Instance.Id}";
    IJtCustomSourceDeclaration IJtCustomSourceParent.Declaration => this;


    void IJtJsonBuildable.BuildJson(StringBuilder sb)
    {
        sb.Append($"\"{Name}\"");
    }
    public override string ToString() => Name;
    IEnumerable<IJtCommonContentElement> IJtCommonParent.EnumerateChildrenElements()
    {
        yield return Value;
    }
    IJtCommonNodeCollection IJtCommonParent.GetChildrenElementsCollection() => throw new NotSupportedException();


    JtNodeSource? IJtNodeSourceParent.Owner => null;

    [MemberNotNull(nameof(identifiersManager))]
    public IdentifiersManager IdentifiersManager => identifiersManager ??= new IdentifiersManager(null);

    CustomSource? IJtCustomSourceDeclaration.Value => (CustomSource)Value;

    bool IJtCommonParent.HasExternalChildrenSource => false;

    public CustomSourceType Type => CustomSourceType.Node;

    IJtCommonNode IJtCommonRoot.CreateNodeElement(IJtCommonParent parent, JtNodeType type) => type.CreateEmptySource((IJtNodeSourceParent)parent);
    IJtCommonNodeCollection IJtCommonRoot.CreateCollectionElement(IJtCommonParent parent) => JtNodeCollectionSource.Create((IJtNodeSourceParent)parent);
}
