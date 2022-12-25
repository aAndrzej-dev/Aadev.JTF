using Aadev.JTF.JtEnumerable;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    internal class CustomSourceBaseDeclaration : ICustomSourceDeclaration
    {
        public CustomSourceBaseDeclaration(JToken source, ICustomSourceProvider sourceProvider)
        {
            SourceProvider = sourceProvider;
            Value = JtNodeSourceCollectionIterator.CreateChildItem(this, source);
        }

        public IJtNodeCollectionSourceChild Value { get; }

        public bool IsDeclaratingSource => Value != null;

        public ICustomSourceProvider SourceProvider { get; }

        ICustomSourceDeclaration ICustomSourceParent.Declaration => this;

        void ICustomSourceDeclaration.BuildJson(StringBuilder sb) => ((CustomSource)Value).BuildJsonDeclaration(sb);
    }
}
