using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface ICustomSourceDeclaration : ICustomSourceParent
    {
        bool IsDeclaratingSource { get; }

        void BuildJson(StringBuilder sb);
    }
}
