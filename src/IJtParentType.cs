namespace Aadev.JTF
{
    public interface IJtParentType
    {
        public JTemplate Template { get; }
        public bool IsInArrayPrefab { get; }
        public bool IsExternal { get; }
        public JtTokenType Type { get; }

        public TokensCollection Children { get; }
    }
}
