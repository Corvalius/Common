using System.Collections.Generic;

namespace Corvalius.Collections
{
    public interface IImmutableList<T> : IEnumerable<T>
    {
        T Head { get; }

        IImmutableList<T> Tail { get; }

        bool IsEmpty { get; }

        bool IsCons { get; }
    }

}
