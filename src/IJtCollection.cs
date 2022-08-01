using System.Text;

namespace Aadev.JTF
{
    public interface IJtCollection
    {
        string? CustomSourceId { get; }
        void BuildJson(StringBuilder sb);
    }
}
