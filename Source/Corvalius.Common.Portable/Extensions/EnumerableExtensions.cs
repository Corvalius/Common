using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> CombineWith<T>(this IEnumerable<T> source, params IEnumerable<T>[] toCombine)
        {
            return source.Concat(toCombine.SelectMany(x => x));
        }
    }
}
