
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Dapper;
using SP.Profanity.Interfaces;
using SP.Profanity.Models;
using SP.Profanity.Utilities;
using SP.Profanity.Helpers;
using Microsoft.Extensions.Logging;

namespace SP.Profanity.Services
{
    public class FilterService : IFilterService
    {
        private readonly IMySqlSettings _mySqlSettings;
        private readonly IWordService _wordService;

        private readonly ILogger<FilterService> _logger;


        public FilterService(IMySqlSettings mySqlSettings, IWordService wordService) => (_mySqlSettings, _wordService) = (mySqlSettings, wordService);

        public FilterService(IMySqlSettings mySqlSettings, IWordService wordService, ILogger<FilterService> plogger) : this(mySqlSettings, wordService) => (_logger) = (plogger);


        public async Task<List<Filter>> GetAllFiltersAsync()
        {
            List<Filter> filters = new List<Filter>();
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"SELECT * FROM filters";
                IEnumerable<Filter> results = await db.QueryAsync<Filter>(sql);
                filters = results.AsList();
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.FilterService.GetAllFiltersAsync: {0}", message);
                throw new Exception($"Unable to get all filters from the database: {message}");
            }
            return filters;
        }

        public async Task<List<Filter>> GetAllFiltersWithWordsAsync()
        {
            List<Filter> filters = new List<Filter>();
            try
            {
                filters = await GetAllFiltersAsync();
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"SELECT w.recordId as wordRecordId, w.word as word, f.recordId as filterRecordId, f.filter as filter, f.description as filterDescription, f.title as filterTitle
                                FROM words w
                                LEFT JOIN filter_words fw ON fw.wordRecordId = w.recordId
                                LEFT JOIN filters f ON f.recordId = fw.filterRecordId
                                ORDER BY f.filter";
                IEnumerable<FilterWordMapping> results = await db.QueryAsync<FilterWordMapping>(sql);
                List<Filter> filtersWithWords = processFiltersWithWords(results.AsList());
                filters.AddOrReplaceRange(filtersWithWords, FilterUtils.ItemIsStringFilter);
                filters.SortByRecordId();
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.FilterService.GetAllFiltersWithWordsAsync: {0}", message);
                throw new Exception($"Unable to get all filters with words from the database: {message}");
            }
            return filters;
        }

        public async Task<Filter> AddFilterAsync(Filter filter)
        {
            Filter createdFilter = await GetFilterWithWordsAsync(filter.filter);
            if (null == createdFilter)
            {
                try
                {
                    using MySqlConnection db = _mySqlSettings.GetConnection();
                    string sql = $@"insert into filters (filter, description, title)
                                    values (@filter, @description, @title)";
                    var affectedRows = await db.ExecuteAsync(sql, new
                    {
                        filter.filter,
                        filter.description,
                        filter.title
                    });
                    if (affectedRows > 0)
                    {
                        createdFilter = await GetFilterWithWordsAsync(filter.filter);
                    }
                }
                catch (Exception ex)
                {
                    string message = ex.ExceptionMessage();
                    _logger?.LogError("SP.Profanity.FilterService.AddFilterAsync: {0}", message);
                    throw new Exception($"Unable to add the filter to the database: {message}");
                }
            }

            return createdFilter;
        }

        public async Task<Filter> DeleteFilterAsync(string filterName)
        {
            Filter retObj = null;
            Filter filterWithId = await GetFilterAsync(filterName);
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                // delete all word associations with this filter.
                string deleteFilterWordsSql = $@"DELETE FROM filter_words
                                                WHERE filterRecordId = (
                                                SELECT recordId FROM filters where filter = @filterName)";
                int deletedFilterWords = await db.ExecuteAsync(deleteFilterWordsSql, new { filterName });
                // delete the filter
                string deleteFilterSql = $@"delete from filters where filter = @filterName;";
                await db.ExecuteAsync(deleteFilterSql, new { filterName });
                // If this fails, it's not a big deal.  It's just here to try to keep the word table cleaned up.
                await _wordService.RemoveOrphanedWordsAsync();
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.FilterService.DeleteFilterAsync: {0}", message);
                throw new Exception($"Unable to delete the filter from the database: {message}");
            }
            retObj = await GetFilterWithWordsAsync(filterName);
            if(retObj == null)
            {
                retObj = new Filter(){
                    recordId = filterWithId.recordId
                };
            }
            return retObj;
        }

        public async Task<Filter> UpdateFilterAsync(string origFilterName, Filter filter)
        {
            Filter updatedFilter = null;
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string sql = $@"update filters
                                set
                                    filter = @filter,
                                    description = @description,
                                    title = @title
                                where filter = @origFilterName;";
                int affectedRows = await db.ExecuteAsync(sql, new
                {
                    filter = filter.filter,
                    description = filter.description,
                    title = filter.title,
                    origFilterName = origFilterName
                });
                if (affectedRows > 0)
                {
                    updatedFilter = await GetFilterWithWordsAsync(filter.filter);
                }
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.FilterService.UpdateFilterAsync: {0}", message);
                throw new Exception($"Unable to update the filter in the database: {message}");
            }
            return updatedFilter;
        }

        public async Task<Filter> GetFilterAsync(string filterName)
        {
            Filter foundFilter = null;
            try
            {
                using MySqlConnection db = _mySqlSettings.GetConnection();
                string filterSql = $@"SELECT *
                                    FROM filters
                                    WHERE filter = @filterName";
                foundFilter = await db.QueryFirstOrDefaultAsync<Filter>(filterSql, new
                {
                    filterName
                });
            }
            catch (Exception ex)
            {
                string message = ex.ExceptionMessage();
                _logger?.LogError("SP.Profanity.FilterService.GetFilterAsync: {0}", message);
                throw new Exception($"Unable to get the filter from the database: {message}");
            }
            return foundFilter;
        }
        
        public async Task<Filter> GetFilterWithWordsAsync(string filterName)
        {
            Filter foundFilter = await GetFilterAsync(filterName);
            if(foundFilter != null){
                List<Word> words = await _wordService.GetAllFilterWordsAsync(filterName);
                if(words.AnyExist()) foundFilter.words = words.ToHashSet();
            }
            return foundFilter;
        }

        #region PrivateMethods
        
        private async Task<List<Filter>> AddWordsToFilterAsync(List<Filter> filters)
        {
            Dictionary<string, Filter> mappedFilters = filters.ToDictionary(f => f.filter);
            List<Word> words = await _wordService.GetAllWordsAsync();
            foreach (Word word in words)
            {
                Word curWord = new Word(word.word, word.recordId);
                foreach (Filter tmpFilter in word.filters)
                {
                    Filter filter = mappedFilters.GetValueOrDefault(tmpFilter.filter);
                    filter.AddWord(curWord);
                }
            }
            return mappedFilters.Values.AsList();
        }
        
        private List<Filter> processFiltersWithWords(IEnumerable<FilterWordMapping> mappings)
        {
            Dictionary<string, Filter> filtersMap = new Dictionary<string, Filter>();
            foreach(FilterWordMapping mapping in mappings)
            {
                Filter filter;
                if (filtersMap.ContainsKey(mapping.filter))
                {
                    filter = filtersMap[mapping.filter];
                }
                else
                {
                    filter = new Filter(mapping.filter, mapping.filterDescription, mapping.filterTitle, mapping.filterRecordId);
                    filtersMap.Add(mapping.filter, filter);
                }
                if (filter.FilterDoesNotContainWord(mapping.word))
                {
                    filter.AddWord(mapping.word, mapping.wordRecordId);
                }
            }
            return filtersMap.Values.AsList();
        }

        #endregion PrivateMethods
    }
}