using Aadev.JTF.CustomSources;
using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtNodeCollectionChild
    {
        internal void BuildJson(StringBuilder sb);
        IEnumerable<JtNode> GetNodes();
        bool IsOverriden();

        IJtNodeCollectionSourceChild CreateSource();
        
        bool IsExternal { get; }
    }
}
