using Aadev.JTF.AbstractStructure;
using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtCustomSourceDeclaration : IJtNodeSourceParent, IJtStructureTemplateElement
    {
        bool IsDeclaringSource { get; }

        internal void BuildJson(StringBuilder sb);
    }
}
