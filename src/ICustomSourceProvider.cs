using Aadev.JTF.CustomSources;

namespace Aadev.JTF
{
    public interface ICustomSourceProvider
    {
       T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource;
       CustomSource? GetCustomSource(JtSourceReference identifier);
    }
}
