using System;
using System.Collections.Generic;

using SP.Profanity.Models;
using SP.Profanity.Helpers;

namespace SP.Profanity.Utilities
{
    public static class FilterUtils
    {
        
        public static bool ListContainsFilter(this IEnumerable<Filter> filterList, string filter)
        {
            return (!string.IsNullOrEmpty(filter)) && filterList.AnyExist(f => f.IsStringFilter(filter));
        }

        public static bool ListContainsFilter(this IEnumerable<Filter> filterList, Filter filter) => filterList.ListContainsFilter(filter?.filter);

        public static bool ListDoesNotContainFilter(this IEnumerable<Filter> filterList, string filter)
        {
            return (!string.IsNullOrEmpty(filter)) && filterList.AllOrDefaultExist(f => !f.IsStringFilter(filter));
        }

        public static bool ListDoesNotContainFilter(this IEnumerable<Filter> filterList, Filter filter)
        {
            return filterList.ListDoesNotContainFilter(filter?.filter);
        }

        public static bool IsStringFilter(this Filter filter, string filterString){
            return filter?.filter?.Equals(filterString) ?? false;
        }

        public static bool IsStringFilter(this Filter a, Filter b) => a.IsStringFilter(b?.filter);

        public static Predicate<Filter> ItemIsStringFilter(this string i) {
            return (Filter f) => f.IsStringFilter(i);
        }

        public static Predicate<Filter> ItemIsStringFilter(this Filter i)  => ItemIsStringFilter(i?.filter);


        public static void SortByRecordId(this List<Filter> filters) => filters.Sort((Filter a, Filter b) => a.recordId.CompareTo(b.recordId));

        public static void SortByKey(this List<Filter> filters) => filters.Sort((Filter a, Filter b) => a.filter.CompareTo(b.filter));

        public static bool FilterContainsWord(this Filter filter, string word) => (filter?.words).ListContainsWord(word);

        public static bool FilterContainsWord(this Filter filter, Word word) => filter.FilterContainsWord(word?.word);
        
        public static bool FilterDoesNotContainWord(this Filter filter, string word) => (filter?.words).ListDoesNotContainWord(word);

        public static bool FilterDoesNotContainWord(this Filter filter, Word word) => filter.FilterDoesNotContainWord(word?.word);

        public static bool AddWord(this Filter filter, Word word) => word != null && (filter?.words).AddExist(word);

        public static bool AddWord(this Filter filter, string word, long recordId = 0) => filter.AddWord(new Word(word, recordId));
    }
}