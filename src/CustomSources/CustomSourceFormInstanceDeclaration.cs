using System.Text;

namespace Aadev.JTF.CustomSources
{
    internal class CustomSourceFormInstanceDeclaration : ICustomSourceDeclaration
    {
        public CustomSourceFormInstanceDeclaration(JtNode instance)
        {
            Instance = instance;
        }

        public JtNode Instance { get; }

        public bool IsDeclaratingSource => false; //TODO

        public ICustomSourceProvider SourceProvider => Instance;

        ICustomSourceDeclaration ICustomSourceParent.Declaration => this;

        void ICustomSourceDeclaration.BuildJson(StringBuilder sb)
        {
            sb.Append($"\"#{Instance.Id}\"");
        }
    }
}
