
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http.Json;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly CategoryApiService _categoryApiService;
        private readonly EventApiService _eventApiService;
        private readonly HttpClient _httpClient;

        public HomeController(
            CategoryApiService categoryApiService,
            EventApiService eventApiService,
            IHttpClientFactory factory)
        {
            _categoryApiService = categoryApiService;
            _eventApiService = eventApiService;
            _httpClient = factory.CreateClient("RaceDayApi");
        }

        // HOME PAGE
        public async Task<IActionResult> Index()
        {
            try
            {
                List<CategoryViewModel> categories =
                    await _categoryApiService.GetCategoriesAsync();

                return View(categories);
            }
            catch (HttpRequestException)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API. Please make sure RaceDay.Api is running.";

                return View(new List<CategoryViewModel>());
            }
            catch (TaskCanceledException)
            {
                ViewBag.ApiError =
                    "The RaceDay API request timed out. Please try again.";

                return View(new List<CategoryViewModel>());
            }
        }

        // EXPLORE RACES
        public async Task<IActionResult> Explore(
            string? search,
            string? country,
            string? raceType)
        {
            try
            {
                List<EventViewModel> events =
                    await _eventApiService.GetEventsAsync();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    events = events.Where(e =>
                        e.EventName.Contains(
                            search,
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!string.IsNullOrWhiteSpace(raceType))
                {
                    events = events.Where(e =>
                        e.EventType.Contains(
                            raceType,
                            StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                ViewBag.Search = search;
                ViewBag.Country = country;
                ViewBag.RaceType = raceType;

                return View(events);
            }
            catch (HttpRequestException)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API. Please try again later.";

                return View(new List<EventViewModel>());
            }
            catch (TaskCanceledException)
            {
                ViewBag.ApiError =
                    "The RaceDay API request timed out. Please try again.";

                return View(new List<EventViewModel>());
            }
        }

        // EVENT DETAILS
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                EventViewModel? selectedEvent =
                    await _eventApiService.GetEventByIdAsync(id);

                if (selectedEvent == null)
                {
                    return NotFound();
                }

                List<CategoryViewModel> categories =
                    await _categoryApiService.GetCategoriesAsync();

                ViewBag.Categories = categories;

                return View(selectedEvent);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    503,
                    "Unable to retrieve event details. Please make sure the RaceDay API is running.");
            }
            catch (TaskCanceledException)
            {
                return StatusCode(
                    504,
                    "The RaceDay API request timed out.");
            }
        }

        // ENROL IN A RACE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enrol(
            int eventId,
            int categoryId)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserId");

            string? role =
                HttpContext.Session.GetString("UserRole");

            if (userId == null || role != "Participant")
            {
                return RedirectToAction("Login", "Account");
            }

            if (eventId <= 0 || categoryId <= 0)
            {
                TempData["EnrolmentError"] =
                    "Please select a valid race category.";

                return RedirectToAction(
                    "Details",
                    new { id = eventId });
            }

            try
            {
                var enrolmentData = new
                {
                    UserId = userId.Value,
                    EventId = eventId,
                    CategoryId = categoryId
                };

                var response =
                    await _httpClient.PostAsJsonAsync(
                        "api/Enrolments",
                        enrolmentData);

                if (response.IsSuccessStatusCode)
                {
                    TempData["EnrolmentSuccess"] =
                        "You have successfully enrolled in this race!";
                }
                else
                {
                    string errorMessage =
                        await response.Content.ReadAsStringAsync();

                    TempData["EnrolmentError"] =
                        string.IsNullOrWhiteSpace(errorMessage)
                        ? "Unable to complete your enrolment."
                        : errorMessage;
                }
            }
            catch (HttpRequestException)
            {
                TempData["EnrolmentError"] =
                    "Unable to connect to the RaceDay API.";
            }
            catch (TaskCanceledException)
            {
                TempData["EnrolmentError"] =
                    "The API request timed out. Please try again.";
            }

            return RedirectToAction(
                "Details",
                new { id = eventId });
        }

        // PRIVACY PAGE
        public IActionResult Privacy()
        {
            return View();
        }

        // ERROR PAGE
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
