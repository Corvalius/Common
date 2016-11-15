using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Collections.Generic
{
    public static class SeriesExtensions
    {
        public static IEnumerable<TType> SplitByRange<TType, TSelector>(this IEnumerable<TType> source, Func<TSelector, TSelector, IEnumerable<TSelector>> rangeConstructor, Func<TType, TSelector> startSelector, Func<TType, TSelector> endSelector, Func<TType, TSelector, TSelector, TType> constructor, IComparer<TSelector> comparer)
        {
            foreach (var o in source)
            {
                TSelector sourceStart = startSelector(o);
                TSelector sourceEnd = endSelector(o);

                var range = rangeConstructor(sourceStart, sourceEnd);

                var timeSerie = range.GetEnumerator();
                if (!timeSerie.MoveNext())
                    throw new ArgumentException("Time serie must have at least 2 items.", "series");

                TSelector first = timeSerie.Current;
                if (!timeSerie.MoveNext())
                    throw new ArgumentException("Time serie must have at least 2 items.", "series");

                do
                {
                    TSelector next = timeSerie.Current;

                    //if (sourceStart >= next || sourceEnd <= first)
                    if (comparer.Compare(sourceStart, next) >= 0 || comparer.Compare(sourceEnd, first) < 0)
                        continue;

                    //if (sourceStart >= first)
                    if (comparer.Compare(sourceStart, first) >= 0)
                    {
                        //if (sourceEnd <= next)
                        if (comparer.Compare(sourceEnd, next) <= 0)
                        {
                            yield return constructor(o, sourceStart, sourceEnd);
                        }
                        else // (sourceEnd > next )
                        {
                            yield return constructor(o, sourceStart, next);
                        }
                    }
                    else // (sourceStart < first )
                    {
                        //if (sourceEnd >= next)
                        if (comparer.Compare(sourceEnd, next) >= 0)
                        {
                            yield return constructor(o, first, next);
                        }
                        else // (sourceEnd < next)
                        {
                            yield return constructor(o, first, sourceEnd);
                        }
                    }

                    first = next;
                }
                while (timeSerie.MoveNext());
            }
        }


        public static IEnumerable<TType> SplitByRange<TType, TSelector>(this IEnumerable<TType> source, IEnumerable<TSelector> range, Func<TType, TSelector> startSelector, Func<TType, TSelector> endSelector, Func<TType, TSelector, TSelector, TType> constructor, IComparer<TSelector> comparer)
        {
            foreach (var o in source)
            {
                TSelector sourceStart = startSelector(o);
                TSelector sourceEnd = endSelector(o);

                var timeSerie = range.GetEnumerator();
                if (!timeSerie.MoveNext())
                    throw new ArgumentException("Time serie must have at least 2 items.", "series");

                TSelector first = timeSerie.Current;
                if (!timeSerie.MoveNext())
                    throw new ArgumentException("Time serie must have at least 2 items.", "series");

                do
                {
                    TSelector next = timeSerie.Current;

                    //if (sourceStart >= next || sourceEnd <= first)
                    if (comparer.Compare(sourceStart, next) >= 0 || comparer.Compare(sourceEnd, first) < 0)
                        continue;

                    //if (sourceStart >= first)
                    if (comparer.Compare(sourceStart, first) >= 0)
                    {
                        //if (sourceEnd <= next)
                        if (comparer.Compare(sourceEnd, next) <= 0)
                        {
                            yield return constructor(o, sourceStart, sourceEnd);
                        }
                        else // (sourceEnd > next )
                        {
                            yield return constructor(o, sourceStart, next);
                        }
                    }
                    else // (sourceStart < first )
                    {
                        //if (sourceEnd >= next)
                        if (comparer.Compare(sourceEnd, next) >= 0)
                        {
                            yield return constructor(o, first, next);
                        }
                        else // (sourceEnd < next)
                        {
                            yield return constructor(o, first, sourceEnd);
                        }
                    }

                    first = next;
                }
                while (timeSerie.MoveNext());
            }
        }
    }
}
