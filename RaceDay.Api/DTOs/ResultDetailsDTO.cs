
namespace RaceDay.Api.DTOs
{
    public class ResultDetailsDTO
    {
        public int ResultID { get; set; }

        public int EnrolmentId { get; set; }

        public int UserID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string ParticipantEmail { get; set; } = string.Empty;

        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string FinishTime { get; set; } = string.Empty;

        public int Position { get; set; }
    }
}
