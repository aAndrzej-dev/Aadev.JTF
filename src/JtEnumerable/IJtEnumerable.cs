using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal interface IJtEnumerable<T> : IEnumerable<T>
    {
        List<T> Enumerate();
    }
}
