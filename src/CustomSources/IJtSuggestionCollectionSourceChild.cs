using System.Text;

namespace Aadev.JTF.CustomSources
{
    internal interface IJtSuggestionCollectionSourceChild<T>
    {
        internal void BuildJson(StringBuilder sb);
        IJtSuggestionCollectionChild<T> CreateInstance();
    }
}
