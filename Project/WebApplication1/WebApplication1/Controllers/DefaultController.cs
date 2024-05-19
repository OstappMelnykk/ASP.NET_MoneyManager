using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class DefaultController : Controller
    {
        ApplicationDbContext db;
        private readonly ILogger<DefaultController> _logger;

        public DefaultController(ApplicationDbContext context, ILogger<DefaultController> logger)
        {
            db = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation($"The default page has been accessed by USER: {User.Identity.Name}");

            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}
