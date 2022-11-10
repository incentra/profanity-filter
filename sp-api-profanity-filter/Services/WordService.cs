using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Dapper;
using SP.Profanity.Helpers;
using SP.Profanity.Interfaces;
using SP.Profanity.Models;
using SP.Profanity.Utilities;
using Microsoft.Extensions.Logging;

namespace SP.Profanity.Services
{
    public class WordService : IWordService
    {
        private readonly IMySqlSettings _mySqlSettings;

        private readonly ILogger<WordService> _logger;

        public WordService(IMySqlSettings mySqlSettings) => (_mySqlSettings) = (mySqlSettings);

        public WordService(IMySqlSettings mySqlSettings, ILogger<WordService> logger): this(mySqlSettings) => (_logger) = (logger);

        public async Task<List<Word>> GetAllWordsAsync()
        {
            List<Word> results = new List<Word>();
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"SELECT w.recordId as wordRecordId, w.word as word, f.recordId as filterRecordId, f.filter as filter, f.description as filterDescription, f.title as filterTitle
                                FROM words w
                                LEFT JOIN filter_words fw ON fw.wordRecordId = w.recordId
                                LEFT JOIN filters f ON f.recordId = fw.filterRecordId
                                ORDER BY w.word";
                IEnumerable<FilterWordMapping> records = await db.QueryAsync<FilterWordMapping>(sql);
                    results = processWordsWithFilters(records.AsList());
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.WordService.GetWordsAsync: {0}", message);
                throw new Exception($"Unable to retrieve words from database: {message}");
            }
            return results;
        }

        public async Task<List<Word>> AddWordsToFiltersAsync(IEnumerable<Filter> filters)
        {
            List<Word> addedWords = new List<Word>();
            IEnumerable<Filter> nonNullFilters = filters?.Where((Filter filter) => !string.IsNullOrEmpty(filter?.filter));
            if(filters.AnyExist())
            {
                foreach(Filter filter in nonNullFilters)
                {
                    foreach(Word word in filter.words)
                    {
                        Word addedWord = await AddWordToFilterAsync(word.word, filter.filter);
                        if(!string.IsNullOrEmpty(addedWord?.word))
                        {
                            addedWords.AddOrEdit(addedWord, addedWord.ItemIsStringWord(), WordUtils.UpdateWordFilters);
                        }
                    }
                }
            }
            addedWords.SortByKey();
            return addedWords;
        }

        public async Task<List<Word>> AddWordsToFiltersAsync(IEnumerable<Word> words, IEnumerable<string> filterNames){
            List<Word> addedWords = new List<Word>();
            IEnumerable<string> filters  = filterNames?.Where(f => !string.IsNullOrEmpty(f));
            if (filters.AnyExist()){
                foreach(Word word in words){
                    addedWords = await AddWordToFilters(addedWords, word.word.ToLower(), filters);
                }
            }
            return addedWords;
        }

        public async Task<List<Word>> AddWordsToFilterAsync(IEnumerable<Word> words, string filter)
        {
            return await AddWordsToFiltersAsync(words, new List<string>(){ filter });
        }

        public async Task<Word> AddWordToFilterAsync(string word, string filter)
        {
            Word addedWord = null;
            string lowerCaseWord = word.ToLower();
            await AddWordAsync(new Word(lowerCaseWord));
            // verify it is not already associated with this filter
            List<Word> filterWords = await GetAllFilterWordsAsync(filter);
            if (!filterWords.ListContainsWord(lowerCaseWord))
            {
                // this is a new word for this filter, so add it to the filter now
                try
                {
                    using MySqlConnection db = _mySqlSettings.GetConnection();
                    string sql = $@"INSERT INTO filter_words (filterRecordId, wordRecordId) 
                                    VALUES (
                                        (SELECT recordId FROM filters WHERE filter = @filter),
                                        (SELECT recordId FROM words WHERE word = @lowerCaseWord))";
                    int affectedRows = await db.ExecuteAsync(sql, new { filter, lowerCaseWord });
                }
                catch (Exception ex)
                {
                    string message = ex.ExceptionMessage();
                    _logger?.LogError("SP.Profanity.DataService.AddWordsToFilterAsync: {0}", message);
                    throw new Exception($"Unable to add word into database: {message}");
                }
            }
            addedWord = await GetWordAsync(lowerCaseWord);
            return addedWord;
        }

        public async Task<bool> RemoveOrphanedWordsAsync()
        {
            // all cases will be considered success unless an exception is thrown
            bool isSuccess = true;
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string deleteOrphanedWordsSql = $@"DELETE FROM words WHERE word IN (
                                                    SELECT * FROM (
                                                        SELECT w1.word
                                                        FROM words w1
                                                        LEFT JOIN filter_words fw1 ON fw1.wordRecordId = w1.recordId
                                                        WHERE fw1.wordRecordId IS NULL
                                                    ) as w
                                                )";
                await db.ExecuteAsync(deleteOrphanedWordsSql);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.FilterService.RemoveOrphanedWordsAsync: {0}", message);
                throw new Exception($"Unable to clean up orphaned words: {message}");
            }
            return isSuccess;
        }

        public async Task<Word> RemoveWordFromFilterAsync(long wordId, string filterName)
        {
            Word retObj = null;
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"DELETE fw
                                FROM filter_words fw
                                INNER JOIN filters f on f.recordId = fw.filterRecordId
                                WHERE f.filter = @filterName AND fw.wordRecordId = @wordId";
                await db.ExecuteAsync(sql, new { filterName, wordId });
                // If this fails, it's not a big deal.  It's just here to try to keep the word table cleaned up.
                await RemoveOrphanedWordsAsync();
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.DataService.RemoveWordFromFilterAsync: {0}", message);
                throw new Exception($"Unable to add word into database: {message}");
            }
            retObj = await GetWordFromIdAsync(wordId);
            retObj ??= new Word(){
                recordId = wordId
            };
            return retObj;
        }

        public async Task<List<Word>> FindDirtyWordsAsync(string text, string filter = Constants.DEFAULT_FILTER)
        {
            HashSet<Word> dirtyWords = new HashSet<Word>();
            try
            {
                List<Word> badWords = new List<Word>();
                badWords = await GetAllFilterWordsAsync(filter);
                IEnumerable<Word> dirtyWordsWithoutSpecialChars = badWords.Where(w => Regex.IsMatch(text.ToLower(), $@"\b{Regex.Escape(w.word.ToLower())}\b"));
                dirtyWords.UnionWith(dirtyWordsWithoutSpecialChars.ToList());
                IEnumerable<Word> dirtyWordsWithSpecialChars = badWords.Where(w => Regex.IsMatch(text.ToLower(), $@"[\s,.;]{Regex.Escape(w.word.ToLower())}[\s,.;]"));
                dirtyWords.UnionWith(dirtyWordsWithSpecialChars.ToList());
                IEnumerable<Word> dirtyWordSingleWord = badWords.Where(w => w.word.Equals(text));
                dirtyWords.UnionWith(dirtyWordSingleWord);
                return dirtyWords.AsList();
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.DataService.FindDirtyWordsAsync: {0}", message);
                throw new Exception($"Unable to find dirty words in text: {message}");
            }
        }

        public async Task<bool> IsCleanAsync(string text, string filter)
        {
            try
            {
                List<Word> badWords = new List<Word>();
                badWords = await FindDirtyWordsAsync(text, filter);
                return !badWords.AnyExist();
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.DataService.IsCleanAsync: {0}", message);
                throw new Exception($"Unable to check if text is clean: {message}");
            }
        }

        public async Task<List<Word>> GetAllFilterWordsAsync(string filter = Constants.DEFAULT_FILTER)
        {
            filter ??= Constants.DEFAULT_FILTER;
            try
            {
                List<Word> results = new List<Word>();
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"SELECT w.*
                                FROM words w
                                INNER JOIN filter_words fw ON fw.wordRecordId = w.recordId
                                INNER JOIN filters f on f.recordId = fw.filterRecordId
                                WHERE f.filter = @filter
                                ORDER BY w.word";
                IEnumerable<Word> records = await db.QueryAsync<Word>(sql, new { filter });
                results = records.AsList();
                return results;
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.DataService.GetAllFilterWordsAsync: {0}", message);
                throw new Exception($"Unable to retrieve words from database: {message}");
            }

        }

        public async Task<List<Word>> BulkAddWordsAsync(Stream file, string filterName)
        {
            return await BulkAddWordsAsync(file, new List<string>{ filterName });
        }

        public async Task<List<Word>> BulkAddWordsAsync(Stream file, IEnumerable<string> filterNames)
        {
            List<Word> addedWords = new List<Word>();
            IEnumerable<string> filters  = filterNames?.Where(f => !string.IsNullOrEmpty(f));
            if (filters.AnyExist())
            {
                try
                {
                    using StreamReader sr = new StreamReader(file);
                    string[] wordList = sr.ReadToEnd().Split('\n');
                    foreach(string word in wordList)
                    {
                        string lowerCaseWord = word.Replace("\r", "").ToLower();
                        addedWords = await AddWordToFilters(addedWords, lowerCaseWord, filters);
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.ExceptionMessage();
                    _logger?.LogError("SP.Profanity.DataService.BulkAddWordsAsync: {0}", message);
                    throw new Exception($"Unable to add words from file to database: {message}");
                }
            }
            return addedWords;
        }

        public async Task<List<Word>> AddWordsToFiltersFromFilesAsync(FilesFilters filesFilters)
        {
            List<Word> addedWords = new List<Word>();
            List<string> filterNames = filesFilters.filters.Select(f => f.filter).Where(f => !string.IsNullOrEmpty(f)).ToList();
            if(filterNames.AnyExist()){
                foreach(IFormFile file in filesFilters.files)
                {
                    List<Word> items = await BulkAddWordsAsync(file.OpenReadStream(), filterNames);
                    addedWords.AddOrEditRange(items, WordUtils.ItemIsStringWord, WordUtils.UpdateWordFilters);
                }
            }
            return addedWords;
        }

        #region PrivateMethods

        private async Task<Word> GetWordAsync(string word)
        {
            List<Word> result = null;
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"SELECT w.recordId as wordRecordId, w.word as word, f.recordId as filterRecordId, f.filter as filter, f.description as filterDescription, f.title as filterTitle
                                FROM words w
                                LEFT JOIN filter_words fw ON fw.wordRecordId = w.recordId
                                LEFT JOIN filters f ON f.recordId = fw.filterRecordId
                                WHERE w.word = @word
                                ORDER BY w.word";
                IEnumerable<FilterWordMapping> record = await db.QueryAsync<FilterWordMapping>(sql, new { word });
                result = processWordsWithFilters(record.AsList());
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.DataService.GetWordAsync: {0}", message);
                throw new Exception($"Unable to retrieve word from database: {message}");
            }
            return result.FirstOrDefault();
        }

        private async Task<Word> GetWordFromIdAsync(long recordId)
        {
            List<Word> result = null;
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"SELECT w.recordId as wordRecordId, w.word as word, f.recordId as filterRecordId, f.filter as filter, f.description as filterDescription, f.title as filterTitle
                                FROM words w
                                LEFT JOIN filter_words fw ON fw.wordRecordId = w.recordId
                                LEFT JOIN filters f ON f.recordId = fw.filterRecordId
                                WHERE w.recordId = @recordId
                                ORDER BY w.word";
                IEnumerable<FilterWordMapping> record = await db.QueryAsync<FilterWordMapping>(sql, new { recordId });
                result = processWordsWithFilters(record.AsList());
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.DataService.GetWordsFromIdAsync: {0}", message);
                throw new Exception($"Unable to retrieve word from database: {message}");
            }
            return result.FirstOrDefault();
        }

        private async Task<List<Word>> AddWordToFilters(List<Word> addedWords, string word, IEnumerable<string> filters){
            foreach(string filterName in filters)
            {
                Word addedWord = await AddWordToFilterAsync(word, filterName);
                if(!string.IsNullOrEmpty(addedWord?.word))
                {
                    addedWords.AddOrEdit(addedWord, addedWord.ItemIsStringWord(), WordUtils.UpdateWordFilters);
                }
            }
            return addedWords;
        }

        private async Task<Word> AddWordAsync(Word word)
        {
            Word addedWord = null;
            string lowerCaseWord = word?.word?.ToLower();
            Word foundWord = await GetWordAsync(lowerCaseWord);
            if (string.IsNullOrEmpty(foundWord?.word))
            {
                try
                {
                    using MySqlConnection db = _mySqlSettings.GetConnection();
                    string sql = "INSERT INTO words (word) VALUES (@lowerCaseWord)";
                    int status = await db.ExecuteAsync(sql, new { lowerCaseWord });
                    addedWord = await GetWordAsync(lowerCaseWord);
                }
                catch (Exception ex)
                {
                    string message = ex.ExceptionMessage();
                    _logger?.LogError("SP.Profanity.DataService.AddWordsAsync: {0}", message);
                    throw new Exception($"Unable to add word into database: {message}");
                }
            }
            return addedWord;
        }
        private List<Word> processWordsWithFilters(IEnumerable<FilterWordMapping> mappings)
        {
            Dictionary<string, Word> wordsMap = new Dictionary<string, Word>();
            foreach(FilterWordMapping mapping in mappings)
            {
                Word word;
                if (wordsMap.ContainsKey(mapping.word))
                {
                    word = wordsMap[mapping.word];
                }
                else
                {
                    word = new Word(mapping.word, mapping.wordRecordId);
                    wordsMap.Add(mapping.word, word);
                }
                if (word.WordDoesNotContainFilter(mapping.filter))
                {
                    word.AddFilter(mapping.filter, mapping.filterDescription, mapping.filterTitle, mapping.filterRecordId);
                }
            }
            return wordsMap.Values.AsList();
        }

        #endregion PrivateMethods

    }
}