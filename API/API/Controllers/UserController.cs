using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Data;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context; // Cambiar ILogger<UserController> por DataContext

        public UserController(DataContext context) // Cambiar el tipo de parámetro
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Events>> GetEvent(Guid id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }
    }
}
