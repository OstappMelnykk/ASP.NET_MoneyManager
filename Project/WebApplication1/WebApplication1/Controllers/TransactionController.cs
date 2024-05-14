using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class TransactionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
