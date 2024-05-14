using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class WalletController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public WalletController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.Accounts = db.Accounts.Where(acc => acc.PersonId == user.Id);
            return View();
        }

        public IActionResult Statistic()
        {
            return View();
        }
    }
}
