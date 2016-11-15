using System.Collections.Generic;

namespace Corvalius.Collections
{
    public interface IImmutableStack<T> : IEnumerable<T>
    {
        IImmutableStack<T> Pop();
        T Peek();
        bool IsEmpty { get; }
    }
}
