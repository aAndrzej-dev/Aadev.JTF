namespace Aadev.JTF.AbstractStructure
{
    public interface IJtStructureNodeElement : IJtStructureInnerElement
    {
        string? Name { get; set; }
        JtNodeType Type { get; }
        bool IsArrayPrefab { get; }
        bool IsRoot { get; }
    }
}
