
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Models
{
    public class EnrolmentDetailsViewModel
    {
        [JsonPropertyName("enrolmentID")]
        public int EnrolmentID { get; set; }

        [JsonPropertyName("userID")]
        public int UserID { get; set; }

        [JsonPropertyName("participantName")]
        public string ParticipantName { get; set; } = string.Empty;

        [JsonPropertyName("participantEmail")]
        public string ParticipantEmail { get; set; } = string.Empty;

        [JsonPropertyName("eventID")]
        public int EventID { get; set; }

        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; } = string.Empty;
    }
}
