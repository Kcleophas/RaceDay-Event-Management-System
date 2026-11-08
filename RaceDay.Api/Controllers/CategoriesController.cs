using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;

namespace RaceDay.Api.Controllers
{
    /// <summary>
    /// Provides endpoints for managing RaceDay race categories.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        // Public: anyone can view categories.
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        // Public: anyone can view an individual category.
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST: api/Categories
        // Only Organisers can create categories.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == category.EventID);

            if (!eventExists)
            {
                return BadRequest("The selected race event does not exist.");
            }

            category.CategoryID = 0;

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                category);
        }

        // PUT: api/Categories/5
        // Only Organisers can update categories.
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.CategoryID)
            {
                return BadRequest("Category ID does not match.");
            }

            bool eventExists = await _context.Events
                .AnyAsync(e => e.EventID == category.EventID);

            if (!eventExists)
            {
                return BadRequest("The selected race event does not exist.");
            }

            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.EventID = category.EventID;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categories/5
        // Only Organisers can delete categories.
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            bool hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.CategoryID == id);

            if (hasEnrolments)
            {
                return Conflict(
                    "This category cannot be deleted because participants are enrolled in it.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
