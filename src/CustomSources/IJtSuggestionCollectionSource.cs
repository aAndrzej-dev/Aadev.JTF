using System.Collections.Generic;
using Aadev.JTF.Common;
using Aadev.JTF.Types;

namespace Aadev.JTF.CustomSources;

public interface IJtSuggestionCollectionSource : IJtCommonSuggestionCollection, ICustomSource
{
    IJtSuggestionCollection CreateInstance(JtValueNode owner);
}
public interface IJtSuggestionCollectionSource<TSuggestion> : IJtSuggestionCollectionSource, IJtSuggestionCollectionSourceChild<TSuggestion>, IList<IJtSuggestionCollectionSourceChild<TSuggestion>>
{

}