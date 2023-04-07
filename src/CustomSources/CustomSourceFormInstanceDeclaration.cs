using Aadev.JTF.AbstractStructure;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public sealed class CustomSourceFormInstanceDeclaration : IJtCustomSourceDeclaration
    {
        private IIdentifiersManager? identifiersManager;

        internal CustomSourceFormInstanceDeclaration(JtNode instance)
        {
            Instance = instance;
        }

        public JtNode Instance { get; }
        public bool IsDeclaringSource => false; //TODO
        public ICustomSourceProvider SourceProvider => Instance;
        public string Name => $"#{Instance.Id}";
        IJtCustomSourceDeclaration IJtCustomSourceParent.Declaration => this;


        void IJtCustomSourceDeclaration.BuildJson(StringBuilder sb)
        {
            sb.Append($"\"{Name}\"");
        }
        public override string ToString() => Name;
        IJtStructureNodeElement IJtStructureTemplateElement.CreateNodeElement(IJtStructureParentElement parent, JtNodeType type) => JtNodeSource.Create((IJtNodeSourceParent)parent, type);
        IEnumerable<IJtStructureInnerElement> IJtStructureTemplateElement.GetStructureChildren()
        {
            yield return Instance;
        }
        JtNodeSource? IJtNodeSourceParent.Owner => null;

        [MemberNotNull(nameof(identifiersManager))]
        public IIdentifiersManager IdentifiersManager => identifiersManager ??= new IdentifiersManager(null); 
        public IJtStructureCollectionElement CreateCollectionElement(IJtStructureParentElement parent) => JtNodeCollectionSource.Create((IJtNodeSourceParent)parent);
    }
}
