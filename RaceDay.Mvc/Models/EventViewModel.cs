
using System;
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Models
{
    public class EventViewModel
    {
        [JsonPropertyName("eventID")]
        public int EventID { get; set; }

        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = string.Empty;

        [JsonPropertyName("eventDate")]
        public DateTime EventDate { get; set; }

        [JsonPropertyName("distanceKm")]
        public double DistanceKm { get; set; }

        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = string.Empty;
    }
}
