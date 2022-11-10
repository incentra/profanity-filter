using System.Collections.Generic;

namespace SP.Profanity.Helpers
{
    public static class ListHelpers
    {

        public static List<T> AddOrEdit<T>(this List<T> list, T item, System.Predicate<T> match, System.Func<T, T, T> editFunc)
        {
            int index = list.FindIndex(match);
            if(index >= 0) list[index] = editFunc(list[index], item);
            else list.Add(item);
            return list;
        }

        public static List<T> AddOrEditRange<T>(this List<T> orgList, List<T> addList, System.Func<T, System.Predicate<T>> matchFunc, System.Func<T, T, T> editFunc)
        {
            addList.ForEach(item => {
                System.Predicate<T> match = matchFunc(item);
                orgList.AddOrEdit(item, match, editFunc);
            });
            return orgList;
        }

        public static List<T> AddOrReplaceRange<T>(this List<T> orgList, List<T> addList, System.Func<T, System.Predicate<T>> matchFunc) =>
            orgList.AddOrEditRange(addList, matchFunc, (i, j) => j);
            
    }

}