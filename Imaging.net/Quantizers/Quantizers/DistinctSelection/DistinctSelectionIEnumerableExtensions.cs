﻿using System;
using System.Collections.Generic;

namespace Imaging.net.Quantizers.DistinctSelection
{
    public static partial class IEnumerableExtensions
    {
        /// <summary>
        /// Selects distinct items by a given selector.
        /// </summary>
        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> items, Func<T, TKey> selector)
        {
            HashSet<TKey> keys = new HashSet<TKey>();
            List<T> values = new List<T>();
            foreach (T value in items)
            {
                if (keys.Add(selector(value)))
                {
                    values.Add(value);
                }
            }
            return values;
        }

        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        public static T MaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, IComparer<TKey> comparer)
        {
            using (IEnumerator<T> sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence was empty");

                T max = sourceIterator.Current;
                TKey maxKey = selector.Invoke(max);

                while (sourceIterator.MoveNext())
                {
                    T candidate = sourceIterator.Current;
                    TKey candidateProjected = selector.Invoke(candidate);

                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }

                return max;
            }
        }
    }
}


