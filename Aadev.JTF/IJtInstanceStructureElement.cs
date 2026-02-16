using System.Collections.Generic;
using Aadev.JTF.Common;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF;

public interface IJtInstanceStructureElement : IJtCommonContentElement
{
    IEnumerable<JtNode> GetNodes();
    bool IsOverridden();
    IJtSourceStructureElement CreateSource();
}
