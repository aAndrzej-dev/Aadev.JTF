using Aadev.JTF.Common;
using Aadev.JTF.Types;

namespace Aadev.JTF.CustomSources;

public interface IJtSuggestionCollectionSourceChild<TSuggestion> : IJtCommonSuggestionCollectionChild
{
    IJtSuggestionCollectionChild<TSuggestion> CreateInstance(JtValueNode owner);
}
