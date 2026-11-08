using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RaceDay.Api.Data;
using RaceDay.Api.DTOs;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        // Public: anyone can view available events.
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GetEvents()
        {
            return Ok(_context.Events.ToList());
        }

        // GET: api/Events/5
        // Public: anyone can view event details.
        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetEvent(int id)
        {
            var evt = _context.Events.Find(id);

            if (evt == null)
                return NotFound();

            return Ok(evt);
        }

        // POST: api/Events
        // Only Organisers can create events.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public IActionResult CreateEvent(EventDTO dto)
        {
            var evt = new Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                DistanceKm = dto.DistanceKm,
                EventType = dto.EventType
            };

            _context.Events.Add(evt);
            _context.SaveChanges();

            return Ok(evt);
        }

        // PUT: api/Events/5
        // Only Organisers can update events.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult UpdateEvent(int id, EventDTO dto)
        {
            var evt = _context.Events.Find(id);

            if (evt == null)
                return NotFound();

            evt.EventName = dto.EventName;
            evt.EventDate = dto.EventDate;
            evt.DistanceKm = dto.DistanceKm;
            evt.EventType = dto.EventType;

            _context.SaveChanges();

            return Ok(evt);
        }

        // DELETE: api/Events/5
        // Only Organisers can delete events.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public IActionResult DeleteEvent(int id)
        {
            var evt = _context.Events.Find(id);

            if (evt == null)
                return NotFound();

            _context.Events.Remove(evt);
            _context.SaveChanges();

            return Ok("Event deleted");
        }
    }
}
