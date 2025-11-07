using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using API.Support;
using API.Data;
namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly Tokens _tokenhandler;
        private readonly DataContext _context;
        public AccountController(Tokens tokenhandler, DataContext context)
        {
            _tokenhandler = tokenhandler;
            _context = context;
        }
        public class LoginRequest
        {
            public required string Username { get; set; }
            public required string Password { get; set; }
        }
        public class RegisterRequest
        {
            public required string Username { get; set; }

            public required string Password { get; set; }
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.Users.SingleOrDefault(u => u.UserName == request.Username);
            
            if (user == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }
            byte[] salt = Convert.FromBase64String(user.Salt!);
            string hash = HashPassword(request.Password, salt, iterationCount: 1000);
            if (user.PasswordHash != hash)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }
            else
            {
                var token = _tokenhandler.GenerateToken(user.Id, user.UserName);
                return Ok(new { Token = token });
            }
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var existingUser = _context.Users.SingleOrDefault(u => u.UserName == request.Username);
            if (existingUser != null)
            {
                return Conflict(new { Message = "Username already exists" });
            }   
            byte[] salt = SaltGenerator();
            string hash =  HashPassword(request.Password, salt, iterationCount: 1000);
            var newUser = new API.Models.Users
            {
                Id = Guid.NewGuid().ToString(),
                UserName = request.Username,
                PasswordHash = hash,
                Salt = Convert.ToBase64String(salt)
            };
            var token = _tokenhandler.GenerateToken(newUser.Id, newUser.UserName);

            _context.Users.Add(newUser);
            _context.SaveChanges(); // or await if you make the method async
            return Ok(new { Message = "User registered successfully", Token=token });
        }
        private static byte[] SaltGenerator()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
        private static string HashPassword(string password, byte[] salt, int iterationCount = 10000, int numBytesRequested = 256 / 8)
        {
            var hashBytes = Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA256,
                iterationCount: iterationCount,
                numBytesRequested: numBytesRequested);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
