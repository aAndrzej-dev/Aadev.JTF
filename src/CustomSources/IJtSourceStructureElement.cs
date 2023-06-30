using Aadev.JTF.Common;
using Newtonsoft.Json.Linq;

namespace Aadev.JTF.CustomSources;

public interface IJtSourceStructureElement : IJtCommonContentElement
{
    IJtInstanceStructureElement CreateInstance(IJtNodeParent parent, JToken? @override);
    IJtSourceStructureElement CreateOverride(IJtNodeSourceParent parent, JToken? @override);
    bool IsOverridden();
}

