namespace Aadev.JTF.CustomSources;

public interface IJtCustomSourceParent : IHaveCustomSourceProvider
{
    IJtCustomSourceDeclaration Declaration { get; }
}
