using Aadev.JTF.AbstractStructure;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtNodeCollectionSourceChild : IJtStructureInnerElement
    {
        internal void BuildJson(StringBuilder sb);
        IJtNodeCollectionChild CreateInstance(IJtNodeParent parent, JToken? @override);
        IJtNodeCollectionSourceChild CreateOverride(IJtNodeSourceParent parent, JToken? @override);
    }
}

