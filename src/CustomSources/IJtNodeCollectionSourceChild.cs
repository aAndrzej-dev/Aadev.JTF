using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    internal interface IJtNodeCollectionSourceChild
    {
        void BuildJson(StringBuilder sb);
        IJtNodeCollectionChild CreateInstance(IJtNodeParent parent, JToken? @override);
    }
}

