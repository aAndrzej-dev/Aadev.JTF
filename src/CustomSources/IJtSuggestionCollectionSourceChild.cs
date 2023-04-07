using System.Text;

namespace Aadev.JTF.CustomSources
{
    public interface IJtSuggestionCollectionSourceChild<T>
    {
        internal void BuildJson(StringBuilder sb);
        IJtSuggestionCollectionChild<T> CreateInstance();
    }
}
