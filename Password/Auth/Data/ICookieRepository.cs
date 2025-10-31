using System;
using Auth.Models;

namespace Auth.Data
{
    public interface ICookieRepository
    {
        Cookies? GetById(string id);
        Cookies? GetByToken(string token);
        IEnumerable<Cookies> GetCookie(DateTimeOffset start, DateTimeOffset end);
        IEnumerable<Cookies> GetLivingCookie();
        //Cookies CreateCookie(int userId, string token, DateTime expiresAt);
        Cookies CreateCookie(Cookies session);
        Cookies BakeCookie(int userId);
        //Cookies UpdateCookie(int id,bool EorK);
        Cookies? UpdateSession(string id, System.Action<Cookies> updateAction);
        Cookies? UpdateCookies(string token, System.Action<Cookies> updateAction);
        Cookies? Update(Cookies updated);
        //Cookies UpdateByToken(string token, Action<Cookies> updateAction);
    }
}
