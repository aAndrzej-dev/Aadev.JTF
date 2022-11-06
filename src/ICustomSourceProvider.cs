using Aadev.JTF.CustomSources;

namespace Aadev.JTF
{
    public interface ICustomSourceProvider
    {
       T? GetCustomSource<T>(JtCustomResourceIdentifier identifier) where T : CustomSource;
       CustomSource? GetCustomSource(JtCustomResourceIdentifier identifier);
    }
}
