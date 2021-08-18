using System.Collections.Generic;
using System;

namespace ConsoleAppHelloWorld.App.MagicOfLinq
{
    public static class Extensions
    {
        public static IEnumerable<T> InterleaveSequenceWith<T>
            (this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstIter = first.GetEnumerator();
            var secondIter = second.GetEnumerator();

            while (firstIter.MoveNext() && secondIter.MoveNext())
            {
                yield return firstIter.Current;
                yield return secondIter.Current;
            }
        }

        public static bool SequenceEquals<T>
            (this IEnumerable<T> first, IEnumerable<T> second)
        {
            var firstIter = first.GetEnumerator();
            var secondIter = second.GetEnumerator();

            while (firstIter.MoveNext() && secondIter.MoveNext())
            {
                var isEqual = firstIter.Current?.Equals(secondIter.Current);
                if (
                    isEqual is null
                    || (isEqual is not null && isEqual is false)
                    )
                {
                    return false;
                }
            }

            return true;
        }

        public static IEnumerable<T> LogQuery<T>
            (this IEnumerable<T> sequence, string tag)
        {
            Console.WriteLine($"Executing Query {tag}");

            return sequence;
        }
    }
}
