using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Aadev.JTF.JtEnumerable
{
    internal abstract class JtIterator<T> : IJtEnumerable<T>, IEnumerator<T>
    {
        private int state;
        private readonly int threadId;
        public T Current { get; protected set; } = default!;
        object IEnumerator.Current => Current!;
        public JtIterator()
        {
            threadId = Environment.CurrentManagedThreadId;
        }
        public abstract JtIterator<T> Clone();
        public virtual IEnumerator<T> GetEnumerator()
        {
            if (state == 4)
                return enumeratedList!.GetEnumerator();
            JtIterator<T> enumerator = state == 0 && threadId == Environment.CurrentManagedThreadId ? this : Clone();
            enumerator.state = 1;
            return enumerator;
        }
        public abstract bool MoveNext();
        public virtual void Reset() => throw new NotSupportedException();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected List<T>? enumeratedList;
        private bool disposedValue;

        public virtual List<T> Enumerate()
        {
            if (enumeratedList != null)
                return enumeratedList;

            enumeratedList = this.ToList();
            state = 4;
            return enumeratedList;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                enumeratedList = null;
                Current = default!;
                state = -1;
                disposedValue = true;
            }
        }

        ~JtIterator()
        {
            Dispose(disposing: false);
        }

        public virtual void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}