using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return (enumerable == null || enumerable.Count() == 0);
        }

        /**
        * Binary search finds item in sorted array.
        * And returns index (zero based) of item
        * If item is not found returns -1
        * Based on C++ example at
        * http://en.wikibooks.org/wiki/Algorithm_implementation/Search/Binary_search#C.2B.2B_.28common_Algorithm.29
        **/
        public static int BinarySearch(this IList<int> array, int value)
        {
            int low = 0, high = array.Count - 1, midpoint = 0;

            while (low <= high)
            {
                midpoint = low + (high - low) / 2;

                // check to see if value is equal to item in array
                if (value == array[midpoint])
                    return midpoint;
                else if (value < array[midpoint])
                    high = midpoint - 1;
                else
                    low = midpoint + 1;
            }

            // item was not found
            return -1;
        }
        
    }
}