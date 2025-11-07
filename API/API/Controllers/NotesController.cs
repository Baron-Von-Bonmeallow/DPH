using API.Data;
using API.Models;
using API.Support;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ILogger<NotesController> _logger;
        private readonly Tokens _token;

        public NotesController(DataContext context, ILogger<NotesController> logger, Tokens token)
        {
            _context = context;
            _logger = logger;
            _token = token;
        }

        private string? GetAuthenticatedUserId()
        {
            var authToken = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authToken)) return null;

            var jwt = _token.IsValidToken(authToken);
            return jwt != null ? _token.GetUserById(jwt) : null;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notes>>> GetNotes()
        {
            return await _context.Notes.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Notes>> GetNote(Guid id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            var note = await _context.Notes.FindAsync(id);
            if (note == null) return NotFound();

            var collaborator = await _context.Collaborators.FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.ResourceId == note.Id &&
                c.ResourceType == "Note");

            if (collaborator == null) return Forbid();

            return Ok(note);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote(Guid id, Notes note)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            if (note.Id == default) note.Id = id;
            if (id != note.Id) return BadRequest();

            var original = await _context.Notes.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
            if (original == null) return NotFound();

            note.CreatedAt = original.CreatedAt;
            note.UpdatedAt = DateTime.UtcNow;
            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Notes>> PostNote(Notes note)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            note.OwnerId = userId;
            note.CreatedAt = DateTime.UtcNow;
            note.UpdatedAt = note.CreatedAt;

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = note.Id }, note);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var userId = GetAuthenticatedUserId();
            if (userId == null) return Unauthorized();

            var note = await _context.Notes.FindAsync(id);
            if (note == null) return NotFound();

            var collaborator = await _context.Collaborators.FirstOrDefaultAsync(c =>
                c.UserId == userId &&
                c.ResourceId == note.Id &&
                c.ResourceType == "Note");

            if (collaborator == null || collaborator.Role != SharedRole.Owner)
                return Forbid();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool NoteExists(Guid id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}