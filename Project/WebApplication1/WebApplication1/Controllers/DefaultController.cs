using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DefaultController : Controller
    {
        ApplicationDbContext db;

        public DefaultController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            ViewData["HostName"] = Environment.MachineName;
            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}
