
namespace RaceDay.Api.DTOs
{
    public class EnrolmentDetailsDTO
    {
        public int EnrolmentID { get; set; }

        public int UserID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string ParticipantEmail { get; set; } = string.Empty;

        public int EventID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;
    }
}
