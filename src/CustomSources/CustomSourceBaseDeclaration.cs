using Aadev.JTF.AbstractStructure;
using Aadev.JTF.CollectionBuilders;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class CustomSourceBaseDeclaration : IJtCustomSourceDeclaration
    {
        private IIdentifiersManager? identifiersManager;
        internal CustomSourceBaseDeclaration(JToken source, ICustomSourceProvider sourceProvider)
        {
            SourceProvider = sourceProvider;
            Value = JtNodeSourceCollectionBuilder.CreateChildItem(this, source);
        }

        public IJtNodeCollectionSourceChild Value { get; }
        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsDeclaringSource => Value is not null;
        public string Name => "Inline Base Declaration";

        public ICustomSourceProvider SourceProvider { get; }

        IJtCustomSourceDeclaration IJtCustomSourceParent.Declaration => this;


        [MemberNotNull(nameof(identifiersManager))]
        public IIdentifiersManager IdentifiersManager => identifiersManager ??= new IdentifiersManager(null);


        JtNodeSource? IJtNodeSourceParent.Owner => null;

        void IJtCustomSourceDeclaration.BuildJson(StringBuilder sb) => ((CustomSource)Value).BuildJsonDeclaration(sb);

        public override string ToString() => Name;
        IJtStructureNodeElement IJtStructureTemplateElement.CreateNodeElement(IJtStructureParentElement parent, JtNodeType type) => JtNodeSource.Create((IJtNodeSourceParent)parent, type);
        IEnumerable<IJtStructureInnerElement> IJtStructureTemplateElement.GetStructureChildren()
        {
            yield return Value;
        }

        public IJtStructureCollectionElement CreateCollectionElement(IJtStructureParentElement parent) => JtNodeCollectionSource.Create((IJtNodeSourceParent)parent);
    }
}
