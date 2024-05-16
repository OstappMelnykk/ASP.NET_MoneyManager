using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public HomeController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }



        [Authorize(Policy = "User")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            bool HasAccount = user.CurrentAccountId != null && db.Accounts.Where(account => account.AccountId == user.CurrentAccountId).Count() != 0;
            decimal Balance = 0m;

            if (HasAccount)
            {
                string accountName = db.Accounts
                    .Where(account => account.AccountId == user.CurrentAccountId)
                    .Select(account => account.Title)
                    .FirstOrDefault();

                List<Transaction> AllTransactions = db.Transactions
                    .Where(
                        transaction => (
                            transaction.AccountFromId == user.CurrentAccountId ||
                            transaction.AccountToId == user.CurrentAccountId))
                    .OrderByDescending(t => t.Date)
                    .ToList();

                List<Transaction> LastTransactions = AllTransactions.Take(3).ToList();

                foreach (var transaction in AllTransactions)
                {
                    if (transaction.AccountToId == user.CurrentAccountId)
                    {
                        Balance += transaction.Sum;
                    }
                    else if (transaction.AccountFromId == user.CurrentAccountId)
                    {
                        Balance -= transaction.Sum;
                    }
                }

                ViewBag.Accounts = db.Accounts.ToList();
                ViewBag.AccountBalance = Balance;
                ViewBag.AccountName = accountName;
                ViewBag.LastTransactions = LastTransactions;
                ViewBag.HasAccount = HasAccount;
            }
            else
            {
                ViewBag.HasAccount = HasAccount;
            }

            return View();
        }

        

    }
}
