using System.Diagnostics;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using NumericSeries.Models;

namespace NumericSeries.Controllers
{
    public class ErrorController: Controller
    {
        [Route("Error")]
        
        public IActionResult Error()
        {
            var expF = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            var EVM= new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Message = expF?.Error.Message ?? "An error occurred while processing your request."
            };
            return View(EVM);
        }
    }
}
