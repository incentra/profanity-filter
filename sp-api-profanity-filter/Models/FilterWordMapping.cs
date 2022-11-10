
namespace SP.Profanity.Models
{
    // Class to handle getting the data for a word and filter that are mapped to each other in database
    public class FilterWordMapping
    {
            public long wordRecordId { get; set; }
            public string word { get; set; }
            public string filter { get; set; }
            public long filterRecordId { get; set; }
            public string filterDescription { get; set; }
            public string filterTitle { get; set ;}
        }
}