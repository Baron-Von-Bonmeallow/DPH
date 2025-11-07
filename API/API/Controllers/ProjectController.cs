using API.Data;
using API.Models;
using API.Support;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens.Experimental;
using System.IdentityModel.Tokens.Jwt;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly Tokens _token;
        public ProjectController(Tokens tokenSupport,DataContext context)
        {
            _token = tokenSupport;
            _context = context;
        }
        public class ShareRequest
        {
            public required string TargetUserId { get; set; }
            public required SharedRole Role { get; set; }
        }
        [Authorize]
        [HttpPost("{projectId}/share")]
        public async Task<IActionResult> ShareProject(Guid projectId, [FromBody] ShareRequest request)
        {
            // Obtener el token del encabezado Authorization
            var token = Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token)) return Unauthorized();

            var jwt = _token.IsValidToken(token);
            if (jwt == null) return Unauthorized();

            var userId = _token.GetUserById(jwt);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var project = await _context.Projects.FindAsync(projectId);
            if (project == null) return NotFound();

            // Verificar si el usuario es Owner o Admin del proyecto
            var projectRole = _context.Collaborators
                .FirstOrDefault(c => c.UserId == userId && c.ResourceId == project.Id && c.ResourceType == "Project")
                ?.Role.ToString();

            if (project.OwnerId != userId && projectRole != "Admin")
            {
                return Forbid("Only the owner or an admin can perform this action.");
            }

            var collaborator = new Collaborators
            {
                UserId = request.TargetUserId,
                ResourceId = projectId,
                ResourceType = "Project",
                Role = request.Role,
                User = await _context.Users.FindAsync(request.TargetUserId) ?? throw new Exception("User not found")
            };

            _context.Collaborators.Add(collaborator);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Project shared successfully." });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] Projects project)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            project.OwnerId = userId;
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetProject),
                new { id = project.Id },
                new { Message = "Proyecto creado exitosamente", ProjectId = project.Id }
            );
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            return Ok(project);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _context.Projects.ToListAsync();
            return Ok(projects);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
