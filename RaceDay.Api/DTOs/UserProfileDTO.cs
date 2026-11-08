
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Api.DTOs
{
    public class UserProfileDTO
    {
        [Required(ErrorMessage = "Full name is required.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
    }
}
