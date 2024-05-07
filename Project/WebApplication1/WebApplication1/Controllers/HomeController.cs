using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public HomeController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}
