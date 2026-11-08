
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDay.Api.Services;

namespace RaceDay.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Participant")]
    public class ProfilePicturesController : ControllerBase
    {
        private readonly BlobStorageService _blobStorageService;
        private readonly IUserService _userService;

        public ProfilePicturesController(
            BlobStorageService blobStorageService,
            IUserService userService)
        {
            _blobStorageService = blobStorageService;
            _userService = userService;
        }

        [HttpPost("upload/{userId}")]
        public async Task<IActionResult> UploadProfilePicture(
            int userId,
            IFormFile file)
        {
            // Get the ID of the logged-in user from the JWT.
            string? loggedInUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(loggedInUserId, out int authenticatedUserId))
            {
                return Unauthorized("Invalid user authentication.");
            }

            // Participants may only upload their own profile pictures.
            if (userId != authenticatedUserId)
            {
                return Forbid();
            }

            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please select an image.");
            }

            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                return BadRequest("Image must not exceed 5 MB.");
            }

            // Check the user exists before uploading.
            var user = _userService.GetUserById(userId);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // IMPORTANT: Save the old blob name before updating the user.
            string? oldBlobName = user.ProfilePictureBlobName;

            string blobName = string.Empty;

            try
            {
                using var inputStream = file.OpenReadStream();
                using var memoryStream = new MemoryStream();

                await inputStream.CopyToAsync(memoryStream);

                byte[] imageBytes = memoryStream.ToArray();

                // Verify the actual file signature.
                string? verifiedContentType =
                    GetVerifiedImageContentType(imageBytes);

                if (verifiedContentType == null)
                {
                    return BadRequest(
                        "Only valid JPG, PNG and WEBP image signatures are accepted.");
                }

                // Use the verified image type, not the supplied filename
                // or Content-Type header.
                string extension = verifiedContentType switch
                {
                    "image/jpeg" => ".jpg",
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    _ => throw new InvalidOperationException(
                        "Unsupported image type.")
                };

                string safeFileName =
                    $"{Guid.NewGuid()}{extension}";

                memoryStream.Position = 0;

                blobName =
                    await _blobStorageService.UploadProfilePictureAsync(
                        memoryStream,
                        safeFileName,
                        verifiedContentType,
                        userId);

                // Save the new blob name in the database.
                var updatedUser =
                    _userService.UpdateProfilePicture(userId, blobName);

                if (updatedUser == null)
                {
                    await _blobStorageService.DeleteProfilePictureAsync(
                        blobName);

                    return NotFound("User not found.");
                }

                // Delete the previous picture only after the new one
                // has been saved successfully.
                if (!string.IsNullOrWhiteSpace(oldBlobName) &&
                    oldBlobName != blobName)
                {
                    try
                    {
                        await _blobStorageService.DeleteProfilePictureAsync(
                            oldBlobName);
                    }
                    catch
                    {
                        // The new picture is already saved.
                        // A failed old-blob deletion should not undo it.
                    }
                }

                return Ok(new
                {
                    message = "Profile picture uploaded successfully.",
                    userId,
                    blobName
                });
            }
            catch (Exception)
            {
                // Clean up the newly uploaded blob if saving fails.
                if (!string.IsNullOrWhiteSpace(blobName))
                {
                    try
                    {
                        await _blobStorageService.DeleteProfilePictureAsync(
                            blobName);
                    }
                    catch
                    {
                        // Cleanup failure.
                    }
                }

                return StatusCode(
                    500,
                    "An error occurred while uploading the profile picture.");
            }
        }

        [HttpGet("view/{userId}")]
        public async Task<IActionResult> GetProfilePicture(int userId)
        {
            string? loggedInUserId =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(loggedInUserId, out int authenticatedUserId))
            {
                return Unauthorized("Invalid user authentication.");
            }

            // Participants may only view their own profile pictures.
            if (userId != authenticatedUserId)
            {
                return Forbid();
            }

            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var user = _userService.GetUserById(userId);

            if (user == null ||
                string.IsNullOrWhiteSpace(user.ProfilePictureBlobName))
            {
                return NotFound("Profile picture not found.");
            }

            var result =
                await _blobStorageService.GetProfilePictureAsync(
                    user.ProfilePictureBlobName);

            if (result.Content == null ||
                result.ContentType == null)
            {
                return NotFound("Profile picture not found.");
            }

            return File(result.Content, result.ContentType);
        }

        // Checks the file's actual signature rather than trusting
        // its filename or supplied Content-Type.
        private static string? GetVerifiedImageContentType(byte[] bytes)
        {
            // JPEG signature
            if (bytes.Length >= 3 &&
                bytes[0] == 0xFF &&
                bytes[1] == 0xD8 &&
                bytes[2] == 0xFF)
            {
                return "image/jpeg";
            }

            // PNG signature
            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 &&
                bytes[1] == 0x50 &&
                bytes[2] == 0x4E &&
                bytes[3] == 0x47 &&
                bytes[4] == 0x0D &&
                bytes[5] == 0x0A &&
                bytes[6] == 0x1A &&
                bytes[7] == 0x0A)
            {
                return "image/png";
            }

            // WEBP signature
            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 &&
                bytes[1] == 0x49 &&
                bytes[2] == 0x46 &&
                bytes[3] == 0x46 &&
                bytes[8] == 0x57 &&
                bytes[9] == 0x45 &&
                bytes[10] == 0x42 &&
                bytes[11] == 0x50)
            {
                return "image/webp";
            }

            return null;
        }
    }
}