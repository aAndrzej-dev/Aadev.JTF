using System.Text;

namespace Aadev.JTF.CustomSources
{
    internal interface IJtSuggestionCollectionSourceChild<in T>
    {
        void BuildJson(StringBuilder sb);
        IJtSuggestionCollectionChild<T> CreateInstance();
    }
}
