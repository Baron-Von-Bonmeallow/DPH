using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SelfCheckOutMarket.Models;

namespace SelfCheckOutMarket.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}