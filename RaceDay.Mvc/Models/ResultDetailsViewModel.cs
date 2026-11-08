
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Models
{
    public class ResultDetailsViewModel
    {
        [JsonPropertyName("resultID")]
        public int ResultID { get; set; }

        [JsonPropertyName("enrolmentId")]
        public int EnrolmentId { get; set; }

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

        [JsonPropertyName("finishTime")]
        public string FinishTime { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public int Position { get; set; }
    }
}
