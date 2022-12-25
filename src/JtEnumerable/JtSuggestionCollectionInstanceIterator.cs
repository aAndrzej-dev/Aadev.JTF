using Aadev.JTF.CustomSources;
using System.Collections.Generic;

namespace Aadev.JTF.JtEnumerable
{
    internal sealed class JtSuggestionCollectionInstanceIterator<T> : JtIterator<IJtSuggestionCollectionChild<T>>
    {
        private readonly JtSuggestionCollectionSource<T> source;
        private IEnumerator<IJtSuggestionCollectionSourceChild<T>>? suggestionEnumerator;
        public JtSuggestionCollectionInstanceIterator(JtSuggestionCollectionSource<T> source)
        {
            this.source = source;
        }

        public override JtIterator<IJtSuggestionCollectionChild<T>> Clone() => new JtSuggestionCollectionInstanceIterator<T>(source);
        public override bool MoveNext()
        {
            suggestionEnumerator ??= source.suggestionEnumerable.Enumerate().GetEnumerator();
            if (!suggestionEnumerator.MoveNext())
            {
                Current = null!;
                return false;
            }
            Current = suggestionEnumerator.Current.CreateInstance();
            return true;
        }
    }
}
