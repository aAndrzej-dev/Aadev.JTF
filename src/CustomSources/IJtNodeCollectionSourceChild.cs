using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtNodeCollectionSourceChild
    {
        internal void BuildJson(StringBuilder sb);
        IJtNodeCollectionChild CreateInstance(IJtNodeParent parent, JToken? @override);
        IJtNodeCollectionSourceChild CreateOverride(ICustomSourceParent parent, JToken? @override);
    }
}

