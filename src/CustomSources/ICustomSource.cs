namespace Aadev.JTF.CustomSources;

public interface ICustomSource
{
    IJtCustomSourceDeclaration Declaration { get; }
    bool IsDeclared { get; }
    ICustomSourceProvider SourceProvider { get; }
}