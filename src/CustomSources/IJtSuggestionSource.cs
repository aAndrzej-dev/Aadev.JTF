using Aadev.JTF.Common;

namespace Aadev.JTF.CustomSources;

public interface IJtSuggestionSource : IJtCommonSuggestion, IJtJsonBuildable
{
}
public interface IJtSuggestionSource<TSuggestion> : IJtSuggestionSource, IJtSuggestionCollectionSourceChild<TSuggestion>
{
    TSuggestion Value { get; set; }
}
