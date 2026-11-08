
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Models
{
    public class CategoryViewModel
    {
        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [JsonPropertyName("eventID")]
        public int EventID { get; set; }
    }
}
