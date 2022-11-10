using System.Collections.Generic;

namespace SP.Profanity.Models
{
    public class Filter
    {
        
        public Filter() {
            this.recordId = 0;
            this.filter = "";
            this.description = "";
            this.title = "";
            this.words = new HashSet<Word>();
        }
        
        public Filter(string filter, string description = "", string title = "", long recordId = 0) {
            this.recordId = recordId;
            this.filter = filter;
            this.description = description;
            this.title = title;
            this.words = new HashSet<Word>();
        }

        public Filter(HashSet<Word> words, string filter = "", string description = "", string title = "", long recordId = 0) {
            this.recordId = recordId;
            this.filter = filter;
            this.description = description;
            this.title = title;
            this.words = words;
        }

        public long recordId { get; set; }
        public string filter { get; set; }
        public string description { get; set; }
        public string title { get; set; }
        
        public HashSet<Word> words { get; set; }
    }
}