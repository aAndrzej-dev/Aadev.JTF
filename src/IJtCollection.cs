using System.Collections;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtCollection : IList
    {
        string? CustomSourceId { get; }
        void BuildJson(StringBuilder sb);
    }
}
