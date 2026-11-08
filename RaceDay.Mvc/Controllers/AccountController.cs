using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;

namespace RaceDay.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("RaceDayApi");
        }

        // LOGIN PAGE
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // PROCESS LOGIN
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "api/Users/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Invalid email address or password.");

                    return View(model);
                }

                var user = await response.Content
                    .ReadFromJsonAsync<LoginResponse>();

                if (user == null || string.IsNullOrWhiteSpace(user.AccessToken))
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to process login. Please try again.");

                    return View(model);
                }

                // Store user information in MVC session
                HttpContext.Session.SetInt32("UserId", user.UserId);

                HttpContext.Session.SetString(
                    "UserName", user.FullName);

                HttpContext.Session.SetString(
                    "UserRole", user.Role);

                // Store JWT access token in server-side session
                HttpContext.Session.SetString(
                    "AccessToken", user.AccessToken);

                return RedirectToAction("Explore", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");

                return View(model);
            }
        }

        // REGISTRATION PAGE
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // PROCESS REGISTRATION
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var registrationData = new
                {
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password,
                    Role = "Participant"
                };

                var response = await _httpClient.PostAsJsonAsync(
                    "api/Users/register", registrationData);

                if (!response.IsSuccessStatusCode)
                {
                    if ((int)response.StatusCode == 409)
                    {
                        ModelState.AddModelError(
                            "Email",
                            "An account with this email already exists.");
                    }
                    else
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            "Registration failed. Please try again.");
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] =
                    "Registration successful! Please log in.";

                return RedirectToAction("Login");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");

                return View(model);
            }
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }

    // API LOGIN RESPONSE
    public class LoginResponse
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string AccessToken { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }
    }
}