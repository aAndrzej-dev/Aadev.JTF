using System.Collections.Generic;

namespace Aadev.JTF.Common;
public interface IJtCommonParent : IJtCommonStructureElement
{
    bool HasExternalChildrenSource { get; }

    IEnumerable<IJtCommonContentElement> EnumerateChildrenElements();
    IJtCommonNodeCollection GetChildrenElementsCollection();
}
