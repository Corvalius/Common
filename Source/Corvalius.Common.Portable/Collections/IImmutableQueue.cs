using System.Collections.Generic;

namespace Corvalius.Collections
{
    public interface IImmutableQueue<T> : IEnumerable<T>
    {
        bool IsEmpty { get; }

        T Peek();

        IImmutableQueue<T> Enqueue(T value);

        IImmutableQueue<T> Dequeue();
    }
}
