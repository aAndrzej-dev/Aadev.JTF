using Newtonsoft.Json.Linq;

namespace Aadev.JTF.Common;

public interface IJtCommonNode : IJtCommonContentElement
{
    string? Name { get; set; }
    string? Description { get; set; }
    string? DisplayName { get; set; }
    string? Condition { get; set; }
    JtIdentifier Id { get; set; }
    bool Required { get; set; }
    bool IsArrayPrefab { get; }
    bool IsDynamicName { get; }
    JtNodeType Type { get; }
    JTokenType JsonType { get; }
    IdentifiersManager IdentifiersManager { get; }
    string GetJson();
}