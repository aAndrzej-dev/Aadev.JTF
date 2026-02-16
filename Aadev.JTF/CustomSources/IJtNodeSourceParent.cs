using Aadev.JTF.Common;

namespace Aadev.JTF.CustomSources;

public interface IJtNodeSourceParent : IJtCustomSourceParent, IJtCommonParent
{
    JtNodeSource? Owner { get; }
}
