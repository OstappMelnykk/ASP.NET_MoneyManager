using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class GoalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
