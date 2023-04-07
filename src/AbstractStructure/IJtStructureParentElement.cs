using System.Collections.Generic;

namespace Aadev.JTF.AbstractStructure
{
    public interface IJtStructureParentElement : IJtStructureInnerElement
    {
        bool HasExternalChildrenSource { get; }
        IJtStructureCollectionElement ChildrenCollection { get; }
        bool IsRoot { get; }
        IEnumerable<IJtStructureInnerElement> GetStructureChildren();
    }
}