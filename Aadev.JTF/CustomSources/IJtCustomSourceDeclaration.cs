using Aadev.JTF.Common;

namespace Aadev.JTF.CustomSources;

public interface IJtCustomSourceDeclaration : IJtNodeSourceParent, IJtCommonRoot
{
    bool IsDeclaringSource { get; }
    CustomSource? Value { get; }
    CustomSourceType Type { get; }
}
