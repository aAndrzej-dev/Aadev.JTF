using Aadev.JTF.CustomSources;

namespace Aadev.JTF.Common;
public interface IJtCommonContentElement : IJtCommonStructureElement
{
    IJtCustomSourceDeclaration? BaseDeclaration { get; }
    bool IsExternal { get; }
    IJtCommonParent Parent { get; }
    IJtCommonRoot Root { get; }
    bool IsRootChild { get; }
}
