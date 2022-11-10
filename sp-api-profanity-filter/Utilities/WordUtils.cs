using System;
using System.Collections.Generic;
using SP.Profanity.Helpers;
using SP.Profanity.Models;

namespace SP.Profanity.Utilities
{
    public static class WordUtils
    {
        
        public static bool ListContainsWord(this IEnumerable<Word> wordList, string word) => (!string.IsNullOrEmpty(word)) && wordList.AnyExist(w => w.IsStringWord(word));

        public static bool ListContainsWord(this IEnumerable<Word> wordList, Word word) => wordList.ListContainsWord(word?.word);

        
        public static bool ListDoesNotContainWord(this IEnumerable<Word> wordList, string word) => (!string.IsNullOrEmpty(word)) && wordList.AllOrDefaultExist(w => !w.IsStringWord(word));

        public static bool ListDoesNotContainsWord(this IEnumerable<Word> wordList, Word word) => wordList.ListDoesNotContainWord(word?.word);

        public static bool IsStringWord(this Word word, string wordString) => word?.word?.Equals(wordString) ?? false;

        public static bool IsStringWord(this Word a, Word b) => a.IsStringWord(b?.word);

        public static Predicate<Word> ItemIsStringWord(this Word i) => ItemIsStringWord(i?.word);

        public static Predicate<Word> ItemIsStringWord(this string i) {
            return (Word w) => w.IsStringWord(i);
        }

        public static void SortByRecordId(this List<Word> words) => words.Sort((Word a, Word b) => a.recordId.CompareTo(b.recordId));

        public static void SortByKey(this List<Word> words) => words.Sort((Word a, Word b) => a.word.CompareTo(b.word));

        public static bool WordContainsFilter(this Word word, string filterString) => (word?.filters).ListContainsFilter(filterString);

        public static bool WordContainsFilter(this Word word, Filter filter) => word.WordContainsFilter(filter);

        public static bool WordDoesNotContainFilter(this Word word, string filterString) => (word?.filters).ListDoesNotContainFilter(filterString);

        public static bool WordDoesNotContainFilter(this Word word, Filter filter) => word.WordDoesNotContainFilter(filter?.filter);

        public static Word UpdateWordFilters(this Word initWord, Word filtersWord){
            initWord.filters = filtersWord.filters;
            return initWord;
        }

        public static bool AddFilter(this Word word, Filter filter) => filter != null && (word?.filters).AddExist(filter);

        public static bool AddFilter(this Word word, string filter, string description = "", string title = "", long recordId = 0)
            => word.AddFilter(new Filter(filter, description, title, recordId));

    }
} 