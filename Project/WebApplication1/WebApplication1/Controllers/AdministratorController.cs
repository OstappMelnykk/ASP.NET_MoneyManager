using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class AdministratorController : Controller
    {

        private readonly ILogger<AdministratorController> _logger;
        
        public AdministratorController(ILogger<AdministratorController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = "Administrator")]
        public IActionResult AdministratorMainPage()
        {
            _logger.LogInformation("Entering AdministratorMainPage method.");
            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            return View();
        }
    }
}
