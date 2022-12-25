namespace Aadev.JTF.CustomSources
{
    public interface ICustomSourceParent
    {
        ICustomSourceDeclaration Declaration { get; }
        ICustomSourceProvider SourceProvider { get; }
    }
}
