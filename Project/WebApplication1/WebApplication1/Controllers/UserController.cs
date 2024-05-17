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

        public UserController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }


        [Authorize(Policy = "User")]
        public async Task<IActionResult> UserMainPage()
        {
            var user = await _userManager.GetUserAsync(User);

            ViewBag.phoneNumber = user.PhoneNumber.ToString();
            ViewBag.email = user.Email.ToString();
            ViewBag.userName = user.UserName.ToString();


            return View();
        }



        
 
        public async Task<IActionResult> Settings(string UserName, string PhoneNumber, string Email)
        {
            var user = await _userManager.GetUserAsync(User);

            if (UserName != null && UserName != "")
            {
                user.UserName = UserName;
                await db.SaveChangesAsync();
            }

            if (PhoneNumber != null && PhoneNumber != "")
            {
                user.PhoneNumber = PhoneNumber;
                await db.SaveChangesAsync();
            }

            if (Email != null && Email != "")
            {
                user.Email = Email;
                await db.SaveChangesAsync();
            }

            return View();
        }
        



    }
}
