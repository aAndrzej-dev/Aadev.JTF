using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    internal class CustomSourceBaseDeclaration : ICustomSourceDeclaration
    {
        public CustomSourceBaseDeclaration(JToken source, ICustomSourceProvider sourceProvider)
        {
            Value = JtNodeSourceCollectionIterator.CreateChildItem(this, source, sourceProvider);
           
        }

        public IJtNodeCollectionSourceChild Value { get; }

        public bool IsDeclaratingSource => Value != null;

        void ICustomSourceDeclaration.BuildJson(StringBuilder sb) => ((CustomSource)Value).BuildJsonDeclaration(sb);
    }
}
