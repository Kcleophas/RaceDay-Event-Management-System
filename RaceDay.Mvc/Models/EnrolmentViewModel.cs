
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Models
{
    public class EnrolmentViewModel
    {
        [JsonPropertyName("enrolmentID")]
        public int EnrolmentID { get; set; }

        [JsonPropertyName("userID")]
        public int UserID { get; set; }

        [JsonPropertyName("eventID")]
        public int EventID { get; set; }

        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }
    }
}
