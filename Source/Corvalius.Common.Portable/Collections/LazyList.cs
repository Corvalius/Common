using System.Collections;
using System.Collections.Generic;

namespace Corvalius.Collections
{
    public class LazyList<T> : IImmutableList<T>
    {
        internal class ConsList : LazyList<T>
        {
            public ConsList(T head, IEnumerator<T> enumerator)
            {
                Head = head;
                this.enumerator = enumerator;
            }
        }

        internal class EmptyList : LazyList<T> { }

        private IImmutableList<T> tail;
        private IEnumerator<T> enumerator;
        private static readonly EmptyList empty = new EmptyList();

        public T Head { get; private set; }

        public static IImmutableList<T> Create(IEnumerator<T> enumerator)
        {
            return enumerator.MoveNext() ? new ConsList(enumerator.Current, enumerator) : Empty;
        }

        public IImmutableList<T> Tail
        {
            get
            {
                if (enumerator != null)
                {
                    tail = Create(enumerator);
                    enumerator = null;
                }
                return tail;
            }
        }

        public bool IsEmpty
        {
            get { return this is EmptyList; }
        }

        public bool IsCons
        {
            get { return this is ConsList; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (IImmutableList<T> current = this; !current.IsEmpty; current = current.Tail)
                yield return current.Head;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static IImmutableList<T> Empty { get { return empty; } }
    }
}
