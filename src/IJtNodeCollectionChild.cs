using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtNodeCollectionChild
    {
        void BuildJson(StringBuilder sb);
        IEnumerable<JtNode> GetNodes();
        bool IsOverriden();
    }
}
