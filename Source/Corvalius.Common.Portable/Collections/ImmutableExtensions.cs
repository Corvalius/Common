
using System;

namespace Corvalius.Collections
{
    public static class ImmutableExtensions
    {
        public static IImmutableStack<T> Push<T>(this IImmutableStack<T> s, T t)
        {
            return ImmutableStack<T>.Push(t, s);
        }

        public static int Length<T>(this IImmutableList<T> list)
        {
            if (list == null)
                return 0;

            return 1 + Length(list.Tail);
        }

        public static int Sum(this IImmutableList<int> list)
        {
            if (list.IsEmpty)
                return 0;

            return list.Head + Sum(list.Tail);
        }

        public static T Last<T>(this IImmutableList<T> items)
        {
            if (items.IsEmpty)
                throw new ArgumentNullException("items");

            var tail = items.Tail;
            return tail.IsEmpty ? items.Head : tail.Last();
        }
    }
}
