using Aadev.JTF.Types;

namespace Aadev.JTF
{
    public interface IJtNodeParent
    {
        JtContainerNode? Owner { get; }
        JTemplate Template { get; }
        bool HasExternalChildren { get; }
        IIdentifiersManager IdentifiersManager { get; }
        ICustomSourceProvider SourceProvider { get; }
        IIdentifiersManager GetIdentifiersManagerForChild();
    }
}
