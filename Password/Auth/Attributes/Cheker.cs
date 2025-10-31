using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Auth.Models;
using Auth.Data; // Asegúrate de que este namespace coincida con donde está tu DbContext
namespace Auth.Attributes
{ 
public class AuthRequiredAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessionId = context.HttpContext.Request.Cookies["SessionId"];
        if (string.IsNullOrEmpty(sessionId))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        var db = context.HttpContext.RequestServices.GetService<ServerContext>();
        var session = db?.Cookies.FirstOrDefault(s => s.Token == sessionId && s.Active);

        if (session == null || session.ExpiresAt < DateTime.UtcNow)
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        session.LastActivity = DateTime.UtcNow;
        session.ExpiresAt = DateTime.UtcNow.AddMinutes(5);
        db?.SaveChanges();

        var user = db?.Users.Find(session.UserId);
        context.HttpContext.Items["User"] = user;
    }
}
}