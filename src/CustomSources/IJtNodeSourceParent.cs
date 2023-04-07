namespace Aadev.JTF.CustomSources
{
    public interface IJtNodeSourceParent : IJtCustomSourceParent
    {
        JtNodeSource? Owner { get; }
    }
}
