namespace Aadev.JTF.CustomSources
{
    public interface IJtCustomSourceParent
    {
        IJtCustomSourceDeclaration Declaration { get; }
        ICustomSourceProvider SourceProvider { get; }
    }
}
