using Auth.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using Auth.Attributes;
namespace Auth.Data
{
    public class CookieRepository : ICookieRepository
    {
        private readonly ServerContext _context;
        public CookieRepository(ServerContext context)
        {
            _context = context;
        }
        public Cookies? GetById(string id)
        {
            return _context.Cookies.FirstOrDefault(c => c.Token == id);
        }
        public Cookies? GetByToken(string token)
        {
            return _context.Cookies.FirstOrDefault(c => c.Token == token);
        }
        public IEnumerable<Cookies> GetCookie(DateTimeOffset start, DateTimeOffset end)
        {
            return _context.Cookies.
                Where(s => s.CreatedAt >= start.DateTime && s.CreatedAt <= end.DateTime)
                .ToList()
                .OrderBy(s=>s.CreatedAt)
                .ToList();
        }
        public IEnumerable<Cookies> GetLivingCookie() 
        {
            return _context.Cookies
                .Where(s=>s.ExpiresAt>=DateTime.Now)
                .ToList();
        }
        public Cookies BakeCookie(int userId)
        {
            var tokenBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }
            string token = Convert.ToBase64String(tokenBytes);

            var session = new Cookies
            {
                Id=token,
                UserId = userId,
                Token = token,
                CreatedAt = DateTime.UtcNow,
                Active = true,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };

            _context.Cookies.Add(session);
            _context.SaveChanges();
            return session;
        }
        public Cookies CreateCookie(Cookies session)
        {
            //var session = new Cookies
            //{
            //    UserId = userId,
            //    Token = token,
            //    CreatedAt = DateTime.UtcNow,
            //    Active = true,
            //    ExpiresAt = expiresAt
            //};

            _context.Cookies.Add(session);
            _context.SaveChanges();
            return session;
        }
        public Cookies? Update(Cookies updated)
        {
            var existing = _context.Cookies.Find(updated.Id);
            if (existing == null) return null;

            existing.Active = updated.Active;
            existing.ExpiresAt = updated.ExpiresAt;

            _context.Cookies.Update(existing);
            _context.SaveChanges();

            return existing;
        }
        public Cookies? UpdateCookies(string token, System.Action<Cookies> updateAction)
        {
            var session = _context.Cookies.FirstOrDefault(c => c.Token == token);
            if (session == null || !session.Active) return null;

            updateAction(session);
            _context.SaveChanges();

            return session;
        }

        public Cookies? UpdateSession(string id, System.Action<Cookies> updateAction)
        {
            var session = _context.Cookies.FirstOrDefault(c => c.Token == id);
            if (session == null || !session.Active) return null;

            updateAction(session);
            _context.SaveChanges();

            return session;
        }
        public Cookies? Extend(string id)
        {
            return UpdateSession(id, s => s.ExpiresAt = DateTime.UtcNow.AddMinutes(5));
        }
    }
}
