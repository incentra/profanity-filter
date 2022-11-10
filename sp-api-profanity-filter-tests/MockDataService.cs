using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Dapper;
using SP.Profanity.Interfaces;
using SP.Profanity.Models;

namespace SP.Profanity.Tests
{
    public class MockDataService : IFilterService
    {

        public async Task<List<Word>> GetWordsAsync(string filter)
        {
            List<Word> profanity = new List<Word>();
            profanity = new List<Word>();
            Word word = new Word();
            await Task.Run(() =>
            {
                word.word = "foobar";
                profanity.Add(word);
            });
            return profanity;
        }

        public async Task<bool> IsCleanAsync(string text, string filterList)
        {
            Boolean isClean = true;
            var wordsToCheck = text.Split(new[] { ' ', '.', ',' }, StringSplitOptions.RemoveEmptyEntries).AsList();
            List<Word> badWords = new List<Word>();
            badWords = await GetWordsAsync(filterList);

            await Task.Run(() =>
            {
                foreach (var word in badWords)
                {
                    if (wordsToCheck.Contains(word.word))
                    {
                        isClean = false;
                        break;
                    }
                }
            });

            return isClean;
        }

        public Task<Word> AddWordAsync(string word, string filterList)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteWordAsync(string word, string filterList)
        {
            throw new NotImplementedException();
        }

        public Task<List<Word>> FindDirtyWordsAsync(string text, string filter = "DEFAULT_SP")
        {
            throw new NotImplementedException();
        }

        public Task<int> BulkAddWordsAsync(Stream file, string filterName)
        {
            throw new NotImplementedException();
        }

        public Task<Filter> AddFilterAsync(Filter filter)
        {
            throw new NotImplementedException();
        }

        public Task<long> DeleteFilterAsync(long recordId)
        {
            throw new NotImplementedException();
        }

        public Task<Filter> UpdateFilterAsync(Filter filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<Word>> GetAllWordsAsync(string filterList = "DEFAULT_SP")
        {
            throw new NotImplementedException();
        }

        public Task<Filter> UpdateFilterAsync(string origFilterName, Filter filter)
        {
            throw new NotImplementedException();
        }

        public Task<List<Filter>> GetAllFiltersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Filter>> GetAllFiltersWithWordsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Filter> DeleteFilterAsync(string filterName)
        {
            throw new NotImplementedException();
        }

        public Task<Filter> GetFilterAsync(string filterName)
        {
            throw new NotImplementedException();
        }

        public Task<Filter> GetFilterWithWordsAsync(string filterName)
        {
            throw new NotImplementedException();
        }
    }
}