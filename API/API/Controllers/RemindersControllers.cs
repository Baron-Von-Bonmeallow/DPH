using API.Data;
using API.Models;
using API.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemindersController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Tokens _tokens;

        public RemindersController(DataContext context, Tokens tokens)
        {
            _context = context;
            _tokens = tokens;
        }

        private string? GetAuthenticatedUserId()
        {
            var token = Request.Headers.Authorization.FirstOrDefault();
            var jwt = _tokens.IsValidToken(token ?? "");
            return jwt != null ? _tokens.GetUserById(jwt) : null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reminders>>> GetReminders()
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            return await _context.Reminders
                .Where(r => r.OwnerId == userId)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Reminders>> GetReminder(Guid id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null) return NotFound();
            if (reminder.OwnerId != userId) return Forbid();

            return reminder;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutReminder(Guid id, Reminders reminder)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            if (reminder.Id == default) reminder.Id = id;
            if (id != reminder.Id) return BadRequest();

            var original = await _context.Reminders.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            if (original == null) return NotFound();
            if (original.OwnerId != userId) return Forbid();

            reminder.OwnerId = userId;
            reminder.UpdatedAt = DateTime.UtcNow;

            _context.Entry(reminder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Reminders>> PostReminder(Reminders reminder)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            reminder.OwnerId = userId;
            reminder.CreatedAt = DateTime.UtcNow;
            reminder.UpdatedAt = reminder.CreatedAt;

            _context.Reminders.Add(reminder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReminder), new { id = reminder.Id }, reminder);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(Guid id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null) return NotFound();
            if (reminder.OwnerId != userId) return Forbid();

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReminderExists(Guid id)
        {
            return _context.Reminders.Any(e => e.Id == id);
        }
    }
}