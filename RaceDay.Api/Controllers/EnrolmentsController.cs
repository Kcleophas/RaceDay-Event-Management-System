
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrolments
        // Only Organisers can view all enrolments.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public IActionResult GetEnrolments()
        {
            var enrolments =
                (from enrolment in _context.Enrolments
                 join user in _context.Users
                    on enrolment.UserID equals user.UserId
                 join raceEvent in _context.Events
                    on enrolment.EventID equals raceEvent.EventID
                 join category in _context.Categories
                    on enrolment.CategoryID equals category.CategoryID
                 select new EnrolmentDetailsDTO
                 {
                     EnrolmentID = enrolment.EnrolmentID,

                     UserID = user.UserId,
                     ParticipantName = user.FullName,
                     ParticipantEmail = user.Email,

                     EventID = raceEvent.EventID,
                     EventName = raceEvent.EventName,

                     CategoryID = category.CategoryID,
                     CategoryName = category.CategoryName
                 }).ToList();

            return Ok(enrolments);
        }

        // GET: api/Enrolments/user/1
        // Participants can only view their own enrolments.
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Participant")]
        public IActionResult GetUserEnrolments(int userId)
        {
            var currentUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserId, out int authenticatedUserId))
            {
                return Unauthorized();
            }

            // Prevent participants from accessing another user's enrolments.
            if (userId != authenticatedUserId)
            {
                return Forbid();
            }

            var enrolments = _context.Enrolments
                .Where(e => e.UserID == authenticatedUserId)
                .ToList();

            return Ok(enrolments);
        }

        // POST: api/Enrolments
        // Only Participants can create enrolments.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public IActionResult CreateEnrolment(EnrolmentDTO dto)
        {
            var currentUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserId, out int authenticatedUserId))
            {
                return Unauthorized();
            }

            // Always use the authenticated user's ID.
            // Never trust the UserId supplied in the request.
            if (!_context.Users.Any(u => u.UserId == authenticatedUserId))
            {
                return NotFound("Participant not found.");
            }

            // Check whether the event exists.
            if (!_context.Events.Any(e => e.EventID == dto.EventId))
            {
                return NotFound("Event not found.");
            }

            // Check whether the category exists and belongs to this event.
            bool categoryIsValid = _context.Categories.Any(c =>
                c.CategoryID == dto.CategoryId &&
                c.EventID == dto.EventId);

            if (!categoryIsValid)
            {
                return BadRequest(
                    "The selected category does not belong to this event.");
            }

            // Prevent duplicate enrolments for the authenticated participant.
            bool alreadyEnrolled = _context.Enrolments.Any(e =>
                e.UserID == authenticatedUserId &&
                e.EventID == dto.EventId);

            if (alreadyEnrolled)
            {
                return Conflict(
                    "You are already enrolled in this event.");
            }

            var enrolment = new Enrolment
            {
                UserID = authenticatedUserId,
                EventID = dto.EventId,
                CategoryID = dto.CategoryId
            };

            _context.Enrolments.Add(enrolment);
            _context.SaveChanges();

            return Ok(enrolment);
        }
    }
}
