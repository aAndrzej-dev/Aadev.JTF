using System.Collections.Generic;

namespace Aadev.JTF.AbstractStructure
{
    public interface IJtStructureTemplateElement : IJtStructureElement
    {
        string Name { get; }

        IJtStructureNodeElement CreateNodeElement(IJtStructureParentElement parent, JtNodeType type);
        IJtStructureCollectionElement CreateCollectionElement(IJtStructureParentElement parent);
        IEnumerable<IJtStructureInnerElement> GetStructureChildren();
    }
}
