﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Corvalius.Collections
{
    public sealed class ImmutableQueue<T> : IImmutableQueue<T>
    {
        private static readonly IImmutableQueue<T> empty = new EmptyQueue();
        private readonly IImmutableStack<T> backwards;
        private readonly IImmutableStack<T> forwards;

        public static IImmutableQueue<T> Empty
        {
            get { return empty; }
        }

        public bool IsEmpty
        {
            get { return false; }
        }
        
        private ImmutableQueue(IImmutableStack<T> f, IImmutableStack<T> b)
        {
            forwards = f;
            backwards = b;
        }

        public T Peek()
        {
            return forwards.Peek();
        }

        public IImmutableQueue<T> Enqueue(T value)
        {
            return new ImmutableQueue<T>(forwards, backwards.Push(value));
        }

        public IImmutableQueue<T> Dequeue()
        {
            IImmutableStack<T> f = forwards.Pop();
            if (!f.IsEmpty)
                return new ImmutableQueue<T>(f, backwards);
            if (backwards.IsEmpty)
                return Empty;
            return new ImmutableQueue<T>((IImmutableStack<T>)backwards.Reverse(), ImmutableStack<T>.Empty);
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var t in forwards) yield return t;
            foreach (var t in backwards.Reverse()) yield return t;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private sealed class EmptyQueue : IImmutableQueue<T>
        {
            public bool IsEmpty
            {
                get { return true; }
            }

            public T Peek()
            {
                throw new InvalidOperationException("empty queue");
            }

            public IImmutableQueue<T> Enqueue(T value)
            {
                return new ImmutableQueue<T>(ImmutableStack<T>.Empty.Push(value), ImmutableStack<T>.Empty);
            }

            public IImmutableQueue<T> Dequeue()
            {
                throw new InvalidOperationException("empty queue");
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
