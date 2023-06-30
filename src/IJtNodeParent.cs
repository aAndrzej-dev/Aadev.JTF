using Aadev.JTF.Common;
using Aadev.JTF.Types;

namespace Aadev.JTF;

public interface IJtNodeParent : IJtCommonParent
{
    JtContainerNode? Owner { get; }
    JtNodeCollection OwnersMainCollection { get; }
    JTemplate Template { get; }
    IdentifiersManager IdentifiersManager { get; }
    IdentifiersManager GetIdentifiersManagerForChild();
}
