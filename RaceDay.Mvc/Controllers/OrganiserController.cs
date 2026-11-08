using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using RaceDay.Mvc.Services;

namespace RaceDay.Mvc.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly OrganiserEventApiService _eventService;
        private readonly CategoryApiService _categoryService;

        public OrganiserController(
            OrganiserEventApiService eventService,
            CategoryApiService categoryService)
        {
            _eventService = eventService;
            _categoryService = categoryService;
        }

        // CHECK ORGANISER ACCESS
        private bool IsOrganiser()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("UserRole");

            return userId != null && role == "Organiser";
        }

        // ORGANISER DASHBOARD
        public IActionResult Dashboard()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.OrganiserName =
                HttpContext.Session.GetString("UserName");

            return View();
        }

        // VIEW ALL EVENTS
        public async Task<IActionResult> Events()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _eventService.GetEventsAsync();
                return View(events);
            }
            catch (HttpRequestException)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API.";

                return View(new List<EventViewModel>());
            }
        }

        // DISPLAY CREATE EVENT FORM
        [HttpGet]
        public IActionResult CreateEvent()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // SAVE NEW EVENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEvent(EventViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool success =
                    await _eventService.CreateEventAsync(model);

                if (success)
                {
                    TempData["SuccessMessage"] =
                        "Race event created successfully.";

                    return RedirectToAction(nameof(Events));
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the event.");

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

        // DISPLAY EDIT EVENT FORM
        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _eventService.GetEventsAsync();

                var selectedEvent = events.FirstOrDefault(
                    e => e.EventID == id);

                if (selectedEvent == null)
                {
                    return NotFound();
                }

                return View(selectedEvent);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    503,
                    "Unable to connect to the RaceDay API.");
            }
        }

        // SAVE EVENT CHANGES
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEvent(
            int id,
            EventViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.EventID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                bool success =
                    await _eventService.UpdateEventAsync(id, model);

                if (success)
                {
                    TempData["SuccessMessage"] =
                        "Race event updated successfully.";

                    return RedirectToAction(nameof(Events));
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the event.");

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

        // DELETE EVENT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                bool success =
                    await _eventService.DeleteEventAsync(id);

                TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                    success
                    ? "Race event deleted successfully."
                    : "Unable to delete the event.";

                return RedirectToAction(nameof(Events));
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to connect to the RaceDay API.";

                return RedirectToAction(nameof(Events));
            }
        }

        // VIEW ALL CATEGORIES
        public async Task<IActionResult> Categories()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var categories =
                    await _categoryService.GetCategoriesAsync();

                return View(categories);
            }
            catch (HttpRequestException)
            {
                ViewBag.ApiError =
                    "Unable to connect to the RaceDay API.";

                return View(new List<CategoryViewModel>());
            }
        }

        // DISPLAY CREATE CATEGORY FORM
        [HttpGet]
        public async Task<IActionResult> CreateCategory()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var events = await _eventService.GetEventsAsync();

                ViewBag.Events = events;

                return View();
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to load race events. Please try again.";

                return RedirectToAction(nameof(Categories));
            }
        }

        // SAVE NEW CATEGORY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(
            CategoryViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                ModelState.AddModelError(
                    nameof(model.CategoryName),
                    "Please enter a category name.");
            }

            if (model.EventID <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.EventID),
                    "Please select a valid race event.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Events = await _eventService.GetEventsAsync();
                return View(model);
            }

            try
            {
                bool success =
                    await _categoryService.CreateCategoryAsync(model);

                if (success)
                {
                    TempData["SuccessMessage"] =
                        "Race category created successfully.";

                    return RedirectToAction(nameof(Categories));
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create the category.");

                ViewBag.Events = await _eventService.GetEventsAsync();
                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");

                ViewBag.Events = await _eventService.GetEventsAsync();
                return View(model);
            }
        }

        // DISPLAY EDIT CATEGORY FORM
        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var category =
                    await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                ViewBag.Events = await _eventService.GetEventsAsync();

                return View(category);
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    503,
                    "Unable to connect to the RaceDay API.");
            }
        }

        // SAVE CATEGORY CHANGES
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(
            int id,
            CategoryViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (id != model.CategoryID)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(model.CategoryName))
            {
                ModelState.AddModelError(
                    nameof(model.CategoryName),
                    "Please enter a category name.");
            }

            if (model.EventID <= 0)
            {
                ModelState.AddModelError(
                    nameof(model.EventID),
                    "Please select a valid race event.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Events = await _eventService.GetEventsAsync();
                return View(model);
            }

            try
            {
                bool success =
                    await _categoryService.UpdateCategoryAsync(id, model);

                if (success)
                {
                    TempData["SuccessMessage"] =
                        "Race category updated successfully.";

                    return RedirectToAction(nameof(Categories));
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update the category.");

                ViewBag.Events = await _eventService.GetEventsAsync();
                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");

                ViewBag.Events = await _eventService.GetEventsAsync();
                return View(model);
            }
        }

        // DELETE CATEGORY
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                bool deleted =
                    await _categoryService.DeleteCategoryAsync(id);

                if (deleted)
                {
                    TempData["SuccessMessage"] =
                        "Category deleted successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Unable to delete category. It may be linked to existing enrolments.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to connect to the RaceDay API.";
            }

            return RedirectToAction(nameof(Categories));
        }
        public async Task<IActionResult> Enrolments()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var enrolmentService =
                    HttpContext.RequestServices
                        .GetRequiredService<EnrolmentApiService>();

                var enrolments =
                    await enrolmentService.GetAllEnrolmentsAsync();

                return View(enrolments);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiError =
                    "API ERROR: " + ex.Message;

                return View(new List<EnrolmentDetailsViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.ApiError =
                    "APPLICATION ERROR: " + ex.Message;

                return View(new List<EnrolmentDetailsViewModel>());
            }
        }
        public async Task<IActionResult> Results()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var resultsService =
                    HttpContext.RequestServices
                        .GetRequiredService<ResultsApiService>();

                var results =
                    await resultsService.GetResultsAsync();

                return View(results);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ApiError =
                    "API ERROR: " + ex.Message;

                return View(new List<ResultDetailsViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.ApiError =
                    "APPLICATION ERROR: " + ex.Message;

                return View(new List<ResultDetailsViewModel>());
            }
        }


        // DISPLAY ADD RESULT FORM
        [HttpGet]
        public async Task<IActionResult> CreateResult()
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var resultsService =
                    HttpContext.RequestServices
                        .GetRequiredService<ResultsApiService>();

                var enrolments =
                    await resultsService.GetEnrolmentsAsync();

                ViewBag.Enrolments = enrolments;

                return View(new ResultCreateViewModel());
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "Unable to load participant enrolments.";

                return RedirectToAction(nameof(Results));
            }
        }

        // SAVE NEW RACE RESULT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateResult(
            ResultCreateViewModel model)
        {
            if (!IsOrganiser())
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    var resultsService =
                        HttpContext.RequestServices
                            .GetRequiredService<ResultsApiService>();

                    ViewBag.Enrolments =
                        await resultsService.GetEnrolmentsAsync();
                }
                catch (HttpRequestException)
                {
                    TempData["ErrorMessage"] =
                        "Unable to load participant enrolments.";

                    return RedirectToAction(nameof(Results));
                }

                return View(model);
            }

            try
            {
                var resultsService =
                    HttpContext.RequestServices
                        .GetRequiredService<ResultsApiService>();

                bool success =
                    await resultsService.CreateResultAsync(model);

                if (success)
                {
                    TempData["SuccessMessage"] =
                        "Race result recorded successfully.";

                    return RedirectToAction(nameof(Results));
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to save the race result.");

                ViewBag.Enrolments =
                    await resultsService.GetEnrolmentsAsync();

                return View(model);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to connect to the RaceDay API.");

                try
                {
                    var resultsService =
                        HttpContext.RequestServices
                            .GetRequiredService<ResultsApiService>();

                    ViewBag.Enrolments =
                        await resultsService.GetEnrolmentsAsync();
                }
                catch (HttpRequestException)
                {
                    TempData["ErrorMessage"] =
                        "Unable to load participant enrolments.";

                    return RedirectToAction(nameof(Results));
                }

                return View(model);
            }
        }

    }
}
