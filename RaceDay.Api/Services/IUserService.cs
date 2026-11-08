
using RaceDay.Api.Models;
using RaceDay.Api.DTOs;

namespace RaceDay.Api.Services
{
    public interface IUserService
    {
        List<User> GetAllUsers();

        User Register(RegisterDTO dto);

        User? Login(LoginDTO dto);

        // Retrieve an individual user's profile
        User? GetUserById(int userId);

        // Update an individual user's profile
        User? UpdateProfile(int userId, UserProfileDTO dto);

        // Save a participant's profile picture blob name
        User? UpdateProfilePicture(int userId, string blobName);
    }
}
