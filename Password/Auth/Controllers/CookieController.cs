using Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using Auth.Attributes;
using Auth.Data;
namespace Auth.Controllers
{
    public class CookieController : Controller
    {
        private readonly ICookieRepository _SessionRepository;
        private readonly IUsersRepository _usersRepository;
        public CookieController(ICookieRepository SessionRepository, IUsersRepository usersRepository)
        {
            _SessionRepository = SessionRepository;
            _usersRepository = usersRepository;
        }
        public Cookies MakeCookie(int userId)
        {
            var currentTime = DateTimeOffset.Now;
            var ExpiresAt = DateTime.UtcNow.AddMinutes(5);
            int id;
            {
                id = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0); // Genera un int único
            }
            byte[] b;
            {
                b= new byte[16];
                using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
                {
                    rng.GetBytes(b);
                }
            }
            string tk= Convert.ToBase64String(b);
            var session = new Cookies
            {
                UserId = userId,
                Token = tk,
                CreatedAt = currentTime.DateTime,
                Active = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            Response.Cookies.Append("SessionId", tk, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = session.ExpiresAt
            });
            return session;
        }
        public ActionResult KillCookie(string id)
        {
            var session = _SessionRepository.GetByToken(id);
            if (session != null)
            {
                session.Active = false;
                session.ExpiresAt = DateTime.UtcNow;
                _SessionRepository.Update(session);
            }
            //Response.Cookies.Delete("SessionId");
            return RedirectToAction("Login", "Account");
        }
        public ActionResult GetCookie(DateTimeOffset start, DateTimeOffset end)
        {
            var sessions = _SessionRepository.GetCookie(start, end);
            return View(sessions);
        }
        public ActionResult GetById(string id)
        {
            var session = _SessionRepository.GetById(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }
        [HttpPost]
        public IActionResult ExtendSession()
        {
            var token = Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login");

            _SessionRepository.UpdateCookies(token, session =>
            {
                session.LastActivity = DateTime.UtcNow;
                session.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
            });

            return Ok();
        }
        public void Update(Cookies sec)
        {
            if (sec.ExpiresAt < DateTime.UtcNow)
            {
                sec.Active = false;
            }
            else
            {
                sec.LastActivity = DateTime.UtcNow;
                sec.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
            }
            _SessionRepository.Update(sec);

        }
        public ActionResult Delete(string id)
        {
            var session = _SessionRepository.GetById(id);
            if (session != null)
            {
                session.Active = false;
                _SessionRepository.Update(session);
            }

            Response.Cookies.Delete("SessionId");
            return RedirectToAction("Login", "Account");
        }
        public ActionResult CookieCheck()
        {
            var token = Request.Cookies["SessionId"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var session = _SessionRepository.GetByToken(token);
            if (session == null || !session.Active || session.ExpiresAt < DateTime.UtcNow)
                return RedirectToAction("Login", "Account");

            session.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
            _SessionRepository.Update(session);

            var user = _usersRepository.GetById(session.UserId);
            HttpContext.Items["User"] = user;
            return RedirectToAction("Protected", "Account");
        }
        [AuthRequired]
        public IActionResult Protected()
        {
            var user = HttpContext.Items["User"] as Users;
            if (user == null)
                return RedirectToAction("Login", "Account");

            return View(user);
        }
    }
}
