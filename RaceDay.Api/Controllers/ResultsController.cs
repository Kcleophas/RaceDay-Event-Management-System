
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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Results
        // Only Organisers can view all original result records.
        [HttpGet]
        [Authorize(Roles = "Organiser")]
        public IActionResult GetResults()
        {
            return Ok(_context.Results.ToList());
        }

        // GET: api/Results/details
        // Organisers can view all detailed results.
        // Participants can view only their own detailed results.
        [HttpGet("details")]
        [Authorize(Roles = "Organiser,Participant")]
        public IActionResult GetDetailedResults()
        {
            var currentUserId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(currentUserId, out int authenticatedUserId))
            {
                return Unauthorized();
            }

            var results =
                from result in _context.Results
                join enrolment in _context.Enrolments
                    on result.EnrolmentId equals enrolment.EnrolmentID
                join user in _context.Users
                    on enrolment.UserID equals user.UserId
                join raceEvent in _context.Events
                    on enrolment.EventID equals raceEvent.EventID
                select new ResultDetailsDTO
                {
                    ResultID = result.ResultID,
                    EnrolmentId = result.EnrolmentId,

                    UserID = user.UserId,
                    ParticipantName = user.FullName,
                    ParticipantEmail = user.Email,

                    EventID = raceEvent.EventID,
                    EventName = raceEvent.EventName,

                    FinishTime = result.FinishTime.ToString(),
                    Position = result.Position
                };

            // Participants only receive their own results.
            if (User.IsInRole("Participant"))
            {
                results = results.Where(r => r.UserID == authenticatedUserId);
            }

            return Ok(results.ToList());
        }

        // POST: api/Results
        // Only Organisers can create race results.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult CreateResult(ResultDTO dto)
        {
            // Check whether the enrolment exists.
            var enrolmentExists = _context.Enrolments
                .Any(e => e.EnrolmentID == dto.EnrolmentId);

            if (!enrolmentExists)
            {
                return NotFound("Enrolment not found.");
            }

            // Validate finishing time.
            if (!TimeSpan.TryParse(dto.FinishTime, out TimeSpan finishTime)
                || finishTime <= TimeSpan.Zero)
            {
                return BadRequest("Please provide a valid finishing time.");
            }

            // Validate finishing position.
            if (dto.Position <= 0)
            {
                return BadRequest("Position must be greater than zero.");
            }

            // Create the result.
            var result = new Result
            {
                EnrolmentId = dto.EnrolmentId,
                FinishTime = finishTime,
                Position = dto.Position
            };

            _context.Results.Add(result);
            _context.SaveChanges();

            return Ok(result);
        }
    }
}
