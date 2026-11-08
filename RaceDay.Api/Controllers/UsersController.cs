using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using RaceDay.Api.Services;
using RaceDay.Api.DTOs;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UsersController(
            IUserService userService,
            IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        // GET: api/Users
        // Only Organisers can view the user list.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public IActionResult GetUsers()
        {
            var users = _userService.GetAllUsers();

            return Ok(users.Select(u => new
            {
                u.UserId,
                u.FullName,
                u.Email,
                u.Role,
                u.ProfilePictureBlobName
            }));
        }

        // POST: api/Users/register
        // Public registration endpoint.
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(RegisterDTO dto)
        {
            try
            {
                var user = _userService.Register(dto);

                return Ok(new
                {
                    Message = "Registration successful.",
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.ProfilePictureBlobName
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // POST: api/Users/login
        // Public login endpoint. Returns a JWT token.
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _userService.Login(dto);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var token = GenerateJwtToken(
                user.UserId,
                user.FullName,
                user.Email,
                user.Role);

            return Ok(new
            {
                Message = "Login successful.",
                AccessToken = token,
                ExpiresAtUtc = DateTime.UtcNow.AddHours(1),
                user.UserId,
                user.FullName,
                user.Email,
                user.Role,
                user.ProfilePictureBlobName
            });
        }

        // Generate JWT access token.
        private string GenerateJwtToken(
            int userId,
            string fullName,
            string email,
            string role)
        {
            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException(
                    "JWT signing key is missing.");

            var jwtIssuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT issuer is missing.");

            var jwtAudience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience is missing.");

            var key = new SymmetricSecurityKey(
                Convert.FromBase64String(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    fullName),

                new Claim(
                    ClaimTypes.Email,
                    email),

                new Claim(
                    ClaimTypes.Role,
                    role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }

        // GET: api/Users/profile/5
        // Authenticated users can only view their own profile.
        [HttpGet("profile/{userId}")]
        [Authorize]
        public IActionResult GetProfile(int userId)
        {
            var currentUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserId, out int authenticatedUserId))
            {
                return Unauthorized();
            }

            if (userId != authenticatedUserId)
            {
                return Forbid();
            }

            var user = _userService.GetUserById(authenticatedUserId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new
            {
                user.UserId,
                user.FullName,
                user.Email,
                user.Role,
                user.ProfilePictureBlobName
            });
        }

        // PUT: api/Users/profile/5
        // Authenticated users can only update their own profile.
        [HttpPut("profile/{userId}")]
        [Authorize]
        public IActionResult UpdateProfile(
            int userId,
            UserProfileDTO dto)
        {
            var currentUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserId, out int authenticatedUserId))
            {
                return Unauthorized();
            }

            if (userId != authenticatedUserId)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = _userService.UpdateProfile(
                    authenticatedUserId,
                    dto);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                return Ok(new
                {
                    Message = "Profile updated successfully.",
                    user.UserId,
                    user.FullName,
                    user.Email,
                    user.Role,
                    user.ProfilePictureBlobName
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
