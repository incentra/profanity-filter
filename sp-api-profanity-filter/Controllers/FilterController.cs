
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SP.Profanity.Interfaces;
using SP.Profanity.Models;

namespace SP.Profanity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : BaseController
    {
        private IFilterService _filterService;

        public FilterController(IFilterService filterService)
        {
            _filterService = filterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiltersAsync()
        {
            return Ok(await _filterService.GetAllFiltersAsync());
        }

        [HttpGet("words/")]
        public async Task<IActionResult> GetAllFiltersWithWordsAsync()
        {
            return Ok(await _filterService.GetAllFiltersWithWordsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> AddFilterAsync([FromBody] Filter filter)
        {
            return Ok(await _filterService.AddFilterAsync(filter));
        }

        [HttpPut("{origFilterName}")]
        public async Task<IActionResult> UpdateFilterAsync(string origFilterName, [FromBody] Filter filter)
        {
            return Ok(await _filterService.UpdateFilterAsync(origFilterName, filter));
        }

        [HttpDelete("{filterName}")]
        public async Task<IActionResult> DeleteFilterAsync(string filterName)
        {
            return Ok(await _filterService.DeleteFilterAsync(filterName));
        }
    }
}
