using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;

        public UserController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }


        [Authorize(Policy = "User")]
        public async Task<IActionResult> UserMainPage()
        {
            return View();
        }
        
        
        
        
        public async Task<IActionResult> Settings()
        {
            return View();
        }
        
        
        public async Task<IActionResult> Security()
        {
            return View();
        }



    }
}
