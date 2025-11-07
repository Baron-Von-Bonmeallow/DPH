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
    public class EventsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Tokens _tokens;
        private readonly ILogger<EventsController> _logger;

        public EventsController(DataContext context, ILogger<EventsController> logger, Tokens tokens)
        {
            _context = context;
            _logger = logger;
            _tokens = tokens;
        }

        private (string? userId, string? role) GetUserInfo()
        {
            var token = Request.Headers.Authorization.FirstOrDefault();
            var jwt = _tokens.IsValidToken(token ?? "");
            if (jwt == null) return (null, null);

            var userId = _tokens.GetUserById(jwt);
            var role = jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            return (userId, role);
        }

        [HttpGet("status")]
        public IActionResult GetStatus() => Ok(new { Status = "Event API is running", Timestamp = DateTime.UtcNow });

        [HttpGet("view")]
        public async Task<IActionResult> ViewEvents()
        {
            var (userId, _) = GetUserInfo();
            if (userId == null) return Unauthorized();

            var events = await _context.Events
                .Where(e =>
                        e.OwnerId == userId ||
                        (e.AdminsIds != null && e.AdminsIds.Contains(userId)) ||
                        (e.EditorsIds != null && e.EditorsIds.Contains(userId)) ||
                        (e.ReadersIds != null && e.ReadersIds.Contains(userId)))
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Events>>> GetEvent()
        {
            var (userId, _) = GetUserInfo();
            if (userId == null) return Unauthorized();

            var events = await _context.Events
                .Where(e =>
                    e.OwnerId == userId ||
                    (e.AdminsIds != null && e.AdminsIds.Contains(userId)) ||
                    (e.EditorsIds != null && e.EditorsIds.Contains(userId)) ||
                    (e.ReadersIds != null && e.ReadersIds.Contains(userId)))
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Events>> GetEvent(Guid id)
        {
            var (userId, _) = GetUserInfo();
            if (userId == null) return Unauthorized();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var hasAccess = @event.OwnerId == userId ||
                            (@event.AdminsIds?.Contains(userId) ?? false) ||
                            (@event.EditorsIds?.Contains(userId) ?? false) ||
                            (@event.ReadersIds?.Contains(userId) ?? false);

            return hasAccess ? Ok(@event) : Forbid();
        }

        [HttpPost]
        public async Task<ActionResult<Events>> PostEvent(Events @event)
        {
            var (userId, _) = GetUserInfo();
            if (userId == null) return Unauthorized();

            if (!IsValid(@event)) return BadRequest("Invalid event data.");

            @event.OwnerId = userId;
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEvent), new { id = @event.Id }, @event);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(Guid id, Events @event)
        {
            var (userId, role) = GetUserInfo();
            if (userId == null) return Unauthorized();
            if (role == "Reader") return Forbid();

            if (@event.Id == default) @event.Id = id;
            if (id != @event.Id) return BadRequest("ID mismatch.");
            if (!IsValid(@event)) return BadRequest("Invalid event data.");

            var original = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (original == null) return NotFound();

            var isAdminOrOwner = role == "Admin" || original.OwnerId == userId;
            if (!isAdminOrOwner && !original.AdminsIds.SequenceEqual(@event.AdminsIds ?? new List<string>()))
                return Forbid();

            @event.StartTime = original.StartTime;
            //@event.UpdatedAt = DateTime.UtcNow;
            _context.Entry(@event).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var (userId, role) = GetUserInfo();
            if (userId == null) return Unauthorized();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var isOwner = @event.OwnerId == userId;
            var isAdmin = @event.AdminsIds?.Contains(userId) ?? false;

            if (role == "Reader" || (!isOwner && !isAdmin))
                return Forbid("Only owners and admins can delete events.");

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(Guid id) => _context.Events.Any(e => e.Id == id);

        private bool IsValid(Events e) =>
            !string.IsNullOrWhiteSpace(e.Title) &&
            !string.IsNullOrWhiteSpace(e.Description) &&
            e.StartTime < e.EndTime;
    }
}