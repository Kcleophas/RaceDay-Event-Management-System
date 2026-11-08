using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly EventApiService _eventApiService;
        private readonly UserProfileApiService _userProfileApiService;

        public ParticipantController(
            IHttpClientFactory factory,
            EventApiService eventApiService,
            UserProfileApiService userProfileApiService)
        {
            _httpClient = factory.CreateClient("RaceDayApi");
            _eventApiService = eventApiService;
            _userProfileApiService = userProfileApiService;
        }

        // ==========================================
        // MY EVENTS
        // ==========================================

        public async Task<IActionResult> MyEvents()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? role = HttpContext.Session.GetString("UserRole");

            if (role != "Participant")
            {
                return Forbid();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"api/Enrolments/user/{userId.Value}");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);

                using var response =
                    await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    ViewBag.ApiError =
                        $"RaceDay API returned {(int)response.StatusCode} " +
                        $"({response.StatusCode}): {errorMessage}";

                    ViewBag.EnrolmentCount = 0;

                    return View(new List<EventViewModel>());
                }

                var enrolments =
                    await response.Content.ReadFromJsonAsync<
                        List<EnrolmentViewModel>>();

                enrolments ??= new List<EnrolmentViewModel>();

                // Retrieve the available race events.
                var events = await _eventApiService.GetEventsAsync();

                // Display only events belonging to this participant.
                var myEvents = events
                    .Where(e => enrolments.Any(en =>
                        en.EventID == e.EventID))
                    .ToList();

                ViewBag.ApiError = null;
                ViewBag.EnrolmentCount = myEvents.Count;

                return View(myEvents);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API: " + ex.Message;

                ViewBag.EnrolmentCount = 0;

                return View(new List<EventViewModel>());
            }
            catch (TaskCanceledException)
            {
                ViewBag.ApiError =
                    "The RaceDay API request timed out.";

                ViewBag.EnrolmentCount = 0;

                return View(new List<EventViewModel>());
            }
        }

        // ==========================================
        // MY RESULTS
        // ==========================================

        public async Task<IActionResult> MyResults()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? role = HttpContext.Session.GetString("UserRole");

            if (role != "Participant")
            {
                return Forbid();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    "api/Results/details");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);

                using var response =
                    await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    ViewBag.ApiError =
                        $"RaceDay API returned {(int)response.StatusCode} " +
                        $"({response.StatusCode}): {errorMessage}";

                    ViewBag.ResultCount = 0;

                    return View(
                        new List<ParticipantResultsViewModel>());
                }

                var allResults =
                    await response.Content.ReadFromJsonAsync<
                        List<ParticipantResultsViewModel>>();

                allResults ??=
                    new List<ParticipantResultsViewModel>();

                // The API already filters results using the JWT.
                // This additional filter provides another safeguard.
                var myResults = allResults
                    .Where(r => r.UserID == userId.Value)
                    .OrderBy(r => r.Position)
                    .ToList();

                ViewBag.ResultCount = myResults.Count;
                ViewBag.ApiError = null;

                return View(myResults);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API: " + ex.Message;

                ViewBag.ResultCount = 0;

                return View(
                    new List<ParticipantResultsViewModel>());
            }
            catch (TaskCanceledException)
            {
                ViewBag.ApiError =
                    "The RaceDay API request timed out.";

                ViewBag.ResultCount = 0;

                return View(
                    new List<ParticipantResultsViewModel>());
            }
        }

        // ==========================================
        // PARTICIPANT PROFILE
        // ==========================================

        public async Task<IActionResult> Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? role = HttpContext.Session.GetString("UserRole");

            if (role != "Participant")
            {
                return Forbid();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var profile = await _userProfileApiService
                    .GetProfileAsync(userId.Value, accessToken);

                if (profile == null)
                {
                    return NotFound();
                }

                return View(profile);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API: " + ex.Message;

                return View(new UserProfileViewModel());
            }
        }

        // Update participant profile.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(
            UserProfileViewModel model)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? role = HttpContext.Session.GetString("UserRole");

            if (role != "Participant")
            {
                return Forbid();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool updated = await _userProfileApiService
                    .UpdateProfileAsync(
                        userId.Value,
                        model,
                        accessToken);

                if (updated)
                {
                    HttpContext.Session.SetString(
                        "UserName",
                        model.FullName);

                    TempData["ProfileSuccess"] =
                        "Your profile has been updated successfully!";

                    return RedirectToAction(nameof(Profile));
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update your profile. Please try again.");

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");

                return View(model);
            }
        }

        // ==========================================
        // UPLOAD PROFILE PICTURE
        // ==========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePicture(
            IFormFile file)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? role = HttpContext.Session.GetString("UserRole");

            if (role != "Participant")
            {
                return Forbid();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login", "Account");
            }

            if (file == null || file.Length == 0)
            {
                TempData["ProfileError"] =
                    "Please select an image.";

                return RedirectToAction(nameof(Profile));
            }

            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                TempData["ProfileError"] =
                    "Image must not exceed 5 MB.";

                return RedirectToAction(nameof(Profile));
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType))
            {
                TempData["ProfileError"] =
                    "Only JPG, PNG and WEBP images are allowed.";

                return RedirectToAction(nameof(Profile));
            }

            try
            {
                bool uploaded = await _userProfileApiService
                    .UploadProfilePictureAsync(
                        userId.Value,
                        file,
                        accessToken);

                if (uploaded)
                {
                    TempData["ProfileSuccess"] =
                        "Your profile picture was uploaded successfully!";
                }
                else
                {
                    TempData["ProfileError"] =
                        "Unable to upload your profile picture.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["ProfileError"] =
                    "Unable to connect to the RaceDay API.";
            }

            return RedirectToAction(nameof(Profile));
        }

        // ==========================================
        // DISPLAY PROFILE PICTURE
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> ProfilePicture()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Unauthorized(
                    "You are not logged in.");
            }

            string? role = HttpContext.Session.GetString("UserRole");

            if (role != "Participant")
            {
                return Forbid();
            }

            string? accessToken =
                HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return Unauthorized(
                    "Your login token is missing. Please log in again.");
            }

            try
            {
                using var request = new HttpRequestMessage(
                    HttpMethod.Get,
                    $"api/ProfilePictures/view/{userId.Value}");

                request.Headers.Authorization =
                    new AuthenticationHeaderValue(
                        "Bearer",
                        accessToken);

                using var response =
                    await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    return StatusCode(
                        (int)response.StatusCode,
                        $"RaceDay API returned {(int)response.StatusCode} " +
                        $"{response.StatusCode}: {errorMessage}");
                }

                var contentType =
                    response.Content.Headers.ContentType?.MediaType;

                if (contentType == null ||
                    !new[]
                    {
                        "image/jpeg",
                        "image/png",
                        "image/webp"
                    }.Contains(contentType))
                {
                    return StatusCode(
                        502,
                        "The API did not return a supported image format.");
                }

                var imageBytes =
                    await response.Content.ReadAsByteArrayAsync();

                return File(imageBytes, contentType);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(
                    502,
                    "Unable to retrieve the profile picture from the API: " +
                    ex.Message);
            }
        }
    }
}
