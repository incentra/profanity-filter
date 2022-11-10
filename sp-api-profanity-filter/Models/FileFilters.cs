using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SP.Profanity.Models
{
    public class FileFilters
    {
        public List<Filter> filters { get; set; }
        public IFormFile file { get; set; }
    }
}