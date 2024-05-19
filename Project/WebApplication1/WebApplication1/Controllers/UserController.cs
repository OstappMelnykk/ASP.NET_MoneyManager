using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(ApplicationDbContext context, UserManager<Person> userManager, ILogger<UserController> logger)
        {
            db = context;
            _userManager = userManager;
            _logger = logger;
        }


        [Authorize(Policy = "User")]
        public async Task<IActionResult> UserMainPage()
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} accessed the UserMainPage method of UserController.");

            ViewBag.phoneNumber = user.PhoneNumber.ToString();
            ViewBag.email = user.Email.ToString();
            ViewBag.userName = user.UserName.ToString();


            return View();
        }



        
 
        public async Task<IActionResult> Settings(string UserName, string PhoneNumber, string Email)
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} accessed the Settings method of UserController.");

            if (UserName != null && UserName != "")
            {
                _logger.LogInformation($"User {user.UserName} updated his username.");
                user.UserName = UserName;
                await db.SaveChangesAsync();
            }

            if (PhoneNumber != null && PhoneNumber != "")
            {
                _logger.LogInformation($"User {user.UserName} updated his phone number.");
                user.PhoneNumber = PhoneNumber;
                await db.SaveChangesAsync();
            }

            if (Email != null && Email != "")
            {
                _logger.LogInformation($"User {user.UserName} updated his email.");
                user.Email = Email;
                await db.SaveChangesAsync();
            }

            return View();
        }
        



    }
}
