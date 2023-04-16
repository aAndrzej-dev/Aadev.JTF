using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtSuggestionCollectionSourceChild<TSuggestion>
    {
        internal void BuildJson(StringBuilder sb);
        IJtSuggestionCollectionChild<TSuggestion> CreateInstance();
    }
}
