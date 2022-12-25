using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtSuggestionCollectionSourceInstanceIterator<T> : JtIterator<IJtSuggestionCollectionSourceChild<T>>
    {
        private readonly JtSuggestionCollection<T> source;
        private readonly ICustomSourceParent parent;
        private IEnumerator<IJtSuggestionCollectionChild<T>>? suggestionEnumerator;
        public JtSuggestionCollectionSourceInstanceIterator(ICustomSourceParent parent, JtSuggestionCollection<T> source)
        {
            this.source = source;
            this.parent = parent;
        }

        public override JtIterator<IJtSuggestionCollectionSourceChild<T>> Clone() => new JtSuggestionCollectionSourceInstanceIterator<T>(parent, source);
        public override bool MoveNext()
        {
            suggestionEnumerator ??= source.suggestionEnumerable.Enumerate().GetEnumerator();
            if (!suggestionEnumerator.MoveNext())
            {
                Current = null!;
                return false;
            }
            Current = suggestionEnumerator.Current.CreateSource(parent);
            return true;
        }
    }
}
