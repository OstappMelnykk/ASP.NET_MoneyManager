using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

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
            var email = user.Email;
            var userId = user.Id;


            ViewBag.Email = email;
            ViewBag.UserId = userId;
           

            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;


            /*bool HasAccount = user.CurrentAccountId != null;

            ViewBag.HasAccount = HasAccount;


            if (HasAccount)
            {

                ViewBag.AccountName = db
                    .Accounts
                    .Where(account => account.AccountId == user.CurrentAccountId).FirstOrDefault().Title;


                ViewBag.AccountBalance = db.Accounts
                    .Where(account => account.AccountId == user.CurrentAccountId)
                    .Select(account => (
                                (account.TransactionsFrom.Select(transaction => transaction.Sum).Sum()) -
                                (account.TransactionsTo.Select(transaction => transaction.Sum).Sum())
                            ));

            }
*/




            return View();
        }
    }
}
