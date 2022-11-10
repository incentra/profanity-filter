

using System.Collections.Generic;
using System.Linq;

namespace SP.Profanity.Helpers
{
    public static class IEnumerableHelpers
    {
        public static bool AnyExist<T>(this IEnumerable<T> sequence) => sequence?.Any() ?? false;

        public static bool AnyExist<T>(this IEnumerable<T> sequence, System.Func<T, bool> predicate) => sequence?.Any(predicate) ?? false;

        public static bool AllOrDefaultExist<T>(this IEnumerable<T> sequence, System.Func<T, bool> predicate, bool nullIsEmpty = false) =>
            sequence?.All(predicate) ?? nullIsEmpty;

    }
}