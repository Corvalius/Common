using System;
using System.Collections;
using System.Collections.Generic;

namespace Corvalius.Collections
{
    public sealed class ImmutableStack<T> : IImmutableStack<T>
    {
        private static readonly EmptyStack empty = new EmptyStack();
        private readonly T head;
        private readonly IImmutableStack<T> tail;

        private ImmutableStack(T head, IImmutableStack<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }

        public static IImmutableStack<T> Empty
        {
            get { return empty; }
        }

        public bool IsEmpty
        {
            get { return false; }
        }

        public T Peek()
        {
            return head;
        }

        public IImmutableStack<T> Pop()
        {
            return tail;
        }

        public static IImmutableStack<T> Push(T head, IImmutableStack<T> tail)
        {
            return new ImmutableStack<T>(head, tail);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (IImmutableStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
                yield return stack.Peek();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private sealed class EmptyStack : IImmutableStack<T>
        {
            public bool IsEmpty
            {
                get { return true; }
            }

            public T Peek()
            {
                throw new InvalidOperationException("Empty stack");
            }

            public IImmutableStack<T> Pop()
            {
                throw new InvalidOperationException("Empty stack");
            }

            public IEnumerator<T> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

    }
}
