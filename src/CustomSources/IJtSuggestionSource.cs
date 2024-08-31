using Aadev.JTF.Common;
using Aadev.JTF.Tools;

namespace Aadev.JTF.CustomSources;

public interface IJtSuggestionSource : IJtCommonSuggestion, IJsonBuildable
{
}
public interface IJtSuggestionSource<TSuggestion> : IJtSuggestionSource, IJtSuggestionCollectionSourceChild<TSuggestion>
{
    TSuggestion Value { get; set; }
}
