using Aadev.JTF.Types;

namespace Aadev.JTF
{
    public interface IJtNodeParent 
    {
        public JtContainer? Owner { get; }
        public JTemplate Template { get; }
        public bool HasExternalChildren { get; }
        public IIdentifiersManager IdentifiersManager { get; }
        public IIdentifiersManager GetIdentifiersManagerForChild();
    }
}
