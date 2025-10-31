using Auth.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Cryptography;
namespace Auth.Data
{
    public class MemoryRepository
    {
        private readonly ConcurrentDictionary<int, Users> _users = new ConcurrentDictionary<int, Users>();
        private readonly ConcurrentDictionary<string, Cookies> _cookies = new ConcurrentDictionary<string, Cookies>();
        //private int _cookieId = 0;
        private int _currentId = 0;
        private readonly object _lockObject = new();
        public Users? GetById(int id)
        {
            _users.TryGetValue(id, out var user);
            return user;
        }
        public Cookies CreateSession(int userId, DateTimeOffset expiresAt)
        {
            byte[] randomBytes = new byte[128/6];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            string tk = Convert.ToBase64String(randomBytes); // Usar el primer byte como Id de la cookie 0-255
            var session = new Cookies
            {
                UserId = userId,
                Token = tk,
                ExpiresAt = expiresAt.DateTime // Conversión explícita de DateTimeOffset a DateTime
            };
            _cookies[session.Token] = session;
            return session;
        }
        public Users Register(Users user)
        {
            if (user == null) { throw new ArgumentNullException(nameof(user)); }
            
            lock(_lockObject)
            {
                user.Id =_currentId++;
                if (!_users.TryAdd(user.Id, user)) { throw new ArgumentException("Unable to Register"); }
            }
            return user;
        }
    }
}
