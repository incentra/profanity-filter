
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SP.Profanity.Interfaces;
using SP.Profanity.Models;

namespace SP.Profanity.Controllers
{
    [Route("api/[controller]")]
    [Route("api/Profanity")]
    [Route("api/FilterWords")]
    [ApiController]
    public class WordController : BaseController
    {
        private IWordService _wordService;

        public WordController(IWordService wordService)
        {
            _wordService = wordService;
        }

        [HttpGet("healthcheck")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok("FilterWords Word API up.");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWordsAsync()
        {
            return Ok(await _wordService.GetAllWordsAsync());
        }

        [HttpPost("filter/{filterName}/isClean")]
        [HttpPost("isClean/{filterName}")]
        [AllowAnonymous]
        public async Task<IActionResult> IsCleanAsync(string filterName, [FromBody] Comment textToCheck)
        {
            return Ok(await _wordService.IsCleanAsync(textToCheck.comment, filterName));
        }

        [HttpGet("filter/{filterName}")]
        [HttpGet("getWords/{filterName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFilterWordsAsync(string filterName)
        {
            return Ok(await _wordService.GetAllFilterWordsAsync(filterName));
        }

        [HttpPost("filter/{filterName}/findDirty")]
        [HttpPost("findDirty/{filterName}")]
        [AllowAnonymous]
        public async Task<ActionResult> FindDirty(string filterName, [FromBody] Comment textToCheck)
        {
            return Ok(await _wordService.FindDirtyWordsAsync(textToCheck.comment, filterName));
        }
    }
}
