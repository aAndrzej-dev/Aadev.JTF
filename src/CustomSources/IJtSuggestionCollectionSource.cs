using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtSuggestionCollectionSource
    {
        bool IsSaveable { get; }

        IJtSuggestionCollection CreateInstance();
        IJtSuggestionCollection CreateInstance(JtCustomResourceIdentifier id);
        void BuildJson(StringBuilder sb);
    }
}
