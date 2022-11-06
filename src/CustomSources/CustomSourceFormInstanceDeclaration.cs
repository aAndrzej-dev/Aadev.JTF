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

        public void BuildJson(StringBuilder sb)
        {
            sb.Append($"\"#{Instance.Id}\"");
        }
    }
}
