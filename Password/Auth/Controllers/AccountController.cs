using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Auth.Data;
using Auth.Controllers;
using Auth.Attributes;
using Auth.ViewModels;
namespace Auth.Controllers
{
    public class AccountController:Controller
    {
        private readonly IUsersRepository _usersRepository;
        private readonly ICookieRepository _cookieRepository;

        public AccountController(IUsersRepository usersRepository, ICookieRepository cookieRepository)
        {
            _usersRepository = usersRepository;
            _cookieRepository = cookieRepository;
        }
        [HttpGet]
        public ActionResult Register() { return View(); }
        [HttpPost]
        public ActionResult Register(RegistryViewModel users) 
        {
            if(!ModelState.IsValid) {return View(users);}
            if (string.IsNullOrEmpty(users.Password))
            {
                ModelState.AddModelError(nameof(users.Password), "La contraseña es obligatoria.");
                return View(users);
            }
            //var new_user = new Users();
            byte[] salt = SaltGeneration();
            string hash = HashPassword(users.Password, salt, iterationCount: 1000);
            var new_user = new Users
            {
                UserName = users.Name,
                Email = users.Email,
                BirthDate = users.BirthDate,
                PasswordHash = hash,
                salt = Convert.ToBase64String(salt)
            };


            _usersRepository.Register(new_user);
            return RedirectToAction("Login", "Account");
        }
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return View();
            }
            Users? user = _usersRepository.GetbyName(model.Username);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View();
            }
            byte[] salt = Convert.FromBase64String(user.salt!);
            string hash = HashPassword(model.Password, salt, iterationCount: 1000);
            if(hash!=user.PasswordHash)
            {
                ModelState.AddModelError(string.Empty, "Incorrect Password");
                return View();
            }

            var session = _cookieRepository.BakeCookie(user.Id);

            Response.Cookies.Append("SessionId", session.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = session.ExpiresAt
            });

            return RedirectToAction("Protected", "Account");
        }
        [HttpGet]
        public ActionResult Login() { return View(); }
        [HttpPost]
        public ActionResult Settings(DateFormat d,int id) 
        {
            if(!ModelState.IsValid) { return View(d); }
            _usersRepository.SettingsChange(id,d);
            return RedirectToAction("Protected", "Account");
        }
        [HttpGet]
        public ActionResult Settings(int id)
        {
            var format = _usersRepository.GetById(id);
            return View(format);
        }
        [AuthRequired]
        public IActionResult Protected()
        {
            var user = HttpContext.Items["User"] as Users;
            if (user == null)
                return RedirectToAction("Login");

            return View(user);
        }
        private static byte[] SaltGeneration() 
        {
            byte[] salt;
            {
                salt = new byte[128 / 8];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
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
