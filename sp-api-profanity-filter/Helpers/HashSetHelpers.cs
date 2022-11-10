
using System.Collections.Generic;
namespace SP.Profanity.Helpers
{
    public static class HashSetHelpers
    {

        public static bool AddExist<T>(this HashSet<T> set, T item) => set?.Add(item) ?? false;
    }
}
