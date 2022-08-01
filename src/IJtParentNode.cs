namespace Aadev.JTF
{
    public interface IJtParentNode
    {
        public IIdentifiersManager IdentifiersManager { get; }
        /// <summary>
        /// Root template
        /// </summary>
        JTemplate Template { get; }
        /// <summary>
        /// <see langword="true"/> if one of parents is <see cref="Types.JtArray"/>
        /// </summary>
        bool IsInArrayPrefab { get; }

        /// <summary>
        /// Type of <see cref="JtNode"/>
        /// </summary>
        JtNodeType Type { get; }

        JtNodeCollection Children { get; }
        bool IsExternal { get; }
    }
}