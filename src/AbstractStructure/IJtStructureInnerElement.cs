using Aadev.JTF.CustomSources;
using System.Diagnostics.CodeAnalysis;

namespace Aadev.JTF.AbstractStructure
{
    public interface IJtStructureInnerElement : IJtStructureElement
    {
        [MemberNotNullWhen(true, nameof(Base))]
        bool IsExternal { get; }
        IJtCustomSourceDeclaration? Base { get; }
    }
}
