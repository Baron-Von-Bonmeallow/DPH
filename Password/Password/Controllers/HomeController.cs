using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Password.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Password.Controllers
{
    public class HomeController:Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            var preferences= GetFromCookies();
            return View(preferences);
        }
        public IActionResult Register(Users U,string password)
        {
            byte[] salt;
            {
                salt = new byte[128 / 8];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
            }
            string hash = HashPassword(password, salt, iterationCount: 10000);
            string saltBase64 = Convert.ToBase64String(salt);
            U.PasswordHash = $"{saltBase64}:{hash}";

            // En una app real aquí persistir U en la base de datos.
            return View(U);
        }
        [HttpPost]
        public IActionResult SetPreferences(Preferences pref)
        { 
            if(ModelState.IsValid)
            {
                var CookiesOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                };
                Response.Cookies.Append("DFormat", pref.DFormat.ToString(), CookiesOptions);
                if (!string.IsNullOrEmpty(pref.UserName))
                {
                    Response.Cookies.Append("UserName", pref.UserName, CookiesOptions);
                }
                Response.Cookies.Append("UserId", pref.UserId.ToString(), CookiesOptions);
                return RedirectToAction("Index");
            }
            return RedirectToAction(nameof(Index));
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
        private static bool VerifyPassword(string password, string storesalt, int iterationCount = 10000, int numBytesRequested = 256 / 8)
        {
            var parts = storesalt.Split(':',2);
            if (string.IsNullOrEmpty(storesalt))
                return false;
            if (parts.Length != 2)
            {
                return false; // Formato inválido
            }
            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];
            var hashToVerify = HashPassword(password, salt, iterationCount, numBytesRequested);
            return hashToVerify == hash;
        }
        public IActionResult Login(Users U,string password)
        {
            bool Pass= VerifyPassword(password, U.PasswordHash!);
            if (!Pass) 
            {
                ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos.");
                return View();
            }

            var CookiesOptions = new CookieOptions
            {
                 HttpOnly = true,
                 Secure = true,
                 SameSite = SameSiteMode.Strict,
                 Expires = DateTimeOffset.UtcNow.AddMinutes(5)
            };
            
            Response.Cookies.Append("AuthUser", U.UserName ?? string.Empty, CookiesOptions);

            return RedirectToAction("Index");
        }
        public IActionResult Logout() 
        {
            Response.Cookies.Delete("AuthUser");
            return View();
        }
        private Preferences GetFromCookies()
        {
            var pref = new Preferences();
            var dformat = Request.Cookies["DFormat"];
            if (Enum.TryParse<DateFormat>(dformat, out var df))
            {
                pref.DFormat = df;
            }
            var userName = Request.Cookies["UserName"];
            if (!string.IsNullOrEmpty(userName))
            {
                pref.UserName = userName;
            }
            var userId = Request.Cookies["UserId"];
            if (int.TryParse(userId, out var uid))
            {
                pref.UserId = uid;
            }
            return pref;
        }
    }
}
