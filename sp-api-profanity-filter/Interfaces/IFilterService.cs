using System.Collections.Generic;
using System.Threading.Tasks;
using SP.Profanity.Models;

namespace SP.Profanity.Interfaces
{
    public interface IFilterService
    {
        Task<List<Filter>> GetAllFiltersAsync();
        Task<List<Filter>> GetAllFiltersWithWordsAsync();
        Task<Filter> AddFilterAsync(Filter filter);
        Task<Filter> UpdateFilterAsync(string origFilterName, Filter filter);
        Task<Filter> DeleteFilterAsync(string filterName);
        Task<Filter> GetFilterAsync(string filterName);
        Task<Filter> GetFilterWithWordsAsync(string filterName);
    }
}