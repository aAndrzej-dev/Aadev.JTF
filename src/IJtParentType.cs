namespace Aadev.JTF
{
    public interface IJtParentType
    {
        JTemplate Template { get; }
        bool IsInArrayPrefab { get; }
        bool IsExternal { get; }
        JtTokenType Type { get; }

        TokensCollection Children { get; }
    }
}
