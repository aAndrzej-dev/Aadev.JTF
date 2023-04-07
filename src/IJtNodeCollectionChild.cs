using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CustomSources;
using System.Collections.Generic;
using System.Text;

namespace Aadev.JTF
{
    public interface IJtNodeCollectionChild : IJtStructureInnerElement
    {
        internal void BuildJson(StringBuilder sb);
        IEnumerable<JtNode> GetNodes();
        bool IsOverridden();

        IJtNodeCollectionSourceChild CreateSource();
    }
}
