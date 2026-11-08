
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class ResultCreateViewModel
    {
        [Required(ErrorMessage = "Please select a participant enrolment.")]
        [Display(Name = "Participant Enrolment")]
        public int EnrolmentId { get; set; }

        [Required(ErrorMessage = "Please enter the finishing time.")]
        [Display(Name = "Finishing Time")]
        public string FinishTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the finishing position.")]
        [Range(1, int.MaxValue,
            ErrorMessage = "Position must be greater than zero.")]
        [Display(Name = "Finishing Position")]
        public int Position { get; set; }
    }
}
