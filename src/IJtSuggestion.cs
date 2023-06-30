using Aadev.JTF.Common;

namespace Aadev.JTF;

public interface IJtSuggestion : IJtCommonSuggestion
{
}
public interface IJtSuggestion<TSuggestion> : IJtSuggestion, IJtSuggestionCollectionChild<TSuggestion>
{
    public TSuggestion Value { get; set; }
}
