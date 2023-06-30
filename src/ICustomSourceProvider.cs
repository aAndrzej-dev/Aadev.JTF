using System.Collections.Generic;
using Aadev.JTF.CustomSources;

namespace Aadev.JTF;

public interface ICustomSourceProvider
{
    IEnumerable<IJtCustomSourceDeclaration> EnumerateCustomSources();
    T? GetCustomSource<T>(JtSourceReference identifier) where T : CustomSource;
    CustomSource? GetCustomSource(JtSourceReference identifier);
}
