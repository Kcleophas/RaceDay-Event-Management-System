
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Models;
using RaceDay.Api.DTOs;
using RaceDay.Api.Data;

namespace RaceDay.Api.Services
{
    public class UserService : IUserService
    {
        private readonly RaceDayDbContext _context;

        public UserService(RaceDayDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User Register(RegisterDTO dto)
        {
            bool emailExists = _context.Users.Any(u =>
                u.Email.ToLower() == dto.Email.ToLower());

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "An account with this email already exists.");
            }

            User user = new User
            {
                FullName = dto.FirstName + " " + dto.LastName,
                Email = dto.Email,
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = "Participant"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User? Login(LoginDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u =>
                u.Email.ToLower() == dto.Email.ToLower());

            if (user == null)
            {
                return null;
            }

            bool valid = BCrypt.Net.BCrypt.Verify(
                dto.Password,
                user.PasswordHash);

            return valid ? user : null;
        }

        public User? GetUserById(int userId)
        {
            return _context.Users
                .FirstOrDefault(u => u.UserId == userId);
        }

        public User? UpdateProfile(
            int userId,
            UserProfileDTO dto)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return null;
            }

            bool emailExists = _context.Users.Any(u =>
                u.Email.ToLower() == dto.Email.ToLower()
                && u.UserId != userId);

            if (emailExists)
            {
                throw new InvalidOperationException(
                    "This email address is already in use.");
            }

            user.FullName = dto.FullName.Trim();
            user.Email = dto.Email.Trim();

            _context.SaveChanges();

            return user;
        }

        // Save the uploaded profile picture blob name
        public User? UpdateProfilePicture(int userId, string blobName)
        {
            var user = _context.Users
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return null;
            }

            user.ProfilePictureBlobName = blobName;

            _context.SaveChanges();

            return user;
        }

    }
}
