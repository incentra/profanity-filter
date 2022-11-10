using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace SP.Profanity.Models
{
    public class FilesFilters
    {
        public List<Filter> filters { get; set; }
        public List<IFormFile> files { get; set; }
    }
}