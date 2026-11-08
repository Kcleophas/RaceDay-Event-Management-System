
namespace RaceDay.Contracts.DTOs
{
    public class EventDTO
    {
        public string EventName { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public double DistanceKm { get; set; }

        public string EventType { get; set; } = string.Empty;
    }
}
