using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SP.Profanity.Helpers;
using SP.Profanity.Models;

namespace SP.Profanity.Interfaces
{
    public interface IWordService
    {
        Task<List<Word>> GetAllWordsAsync();
        Task<bool> RemoveOrphanedWordsAsync();
        Task<Word> AddWordToFilterAsync(string word, string filterName);
        Task<List<Word>> AddWordsToFiltersAsync(IEnumerable<Filter> filters);
        Task<List<Word>> AddWordsToFilterAsync(IEnumerable<Word> words, string filter);
        Task<Word> RemoveWordFromFilterAsync(long wordId, string filterName = Constants.DEFAULT_FILTER);
        Task<List<Word>> GetAllFilterWordsAsync(string filterName = Constants.DEFAULT_FILTER);
        Task<List<Word>> FindDirtyWordsAsync(string text, string filter = Constants.DEFAULT_FILTER);
        Task<bool> IsCleanAsync(string text, string filterName);
        Task<List<Word>> BulkAddWordsAsync(Stream file, string filterName);
        Task<List<Word>> BulkAddWordsAsync(Stream file, IEnumerable<string> filterNames);
        Task<List<Word>> AddWordsToFiltersFromFilesAsync(FilesFilters filesFilters);
    }
}