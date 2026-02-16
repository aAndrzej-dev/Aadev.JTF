namespace Aadev.JTF.Common;
public interface IJtCommonRoot : IJtCommonParent
{
    string Name { get; }
    IJtCommonNode CreateNodeElement(IJtCommonParent parent, JtNodeType type);
    IJtCommonNodeCollection CreateCollectionElement(IJtCommonParent parent);
}
