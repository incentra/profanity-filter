using System.Collections.Generic;

namespace SP.Profanity.Models
{
    public class Word
    {
        public Word() {
            this.recordId = 0;
            this.word = "";
            this.filters = new HashSet<Filter>();
        }
        
        public Word(string word, long recordId = 0) {
            this.recordId = recordId;
            this.word = word;
            this.filters = new HashSet<Filter>();
        }

        public Word(HashSet<Filter> filters, string word = "", long recordId = 0) {
            this.recordId = recordId;
            this.word = word;
            this.filters = filters;
        }

        public long recordId { get; set; }
        public string word { get; set; }
        public HashSet<Filter> filters { get; set; }
    }
}