
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RaceDay.Mvc.Models
{
    public class UserProfileViewModel
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter your full name.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        [JsonPropertyName("profilePictureBlobName")]
        public string? ProfilePictureBlobName { get; set; }
    }
}
