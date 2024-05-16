using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public TransactionController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);


            bool HasAccount = user.CurrentAccountId != null;
            decimal Balance = 0m;

            if (HasAccount)
            {       

                List<Transaction> AllTransactions = db.Transactions
                    .Where(
                        transaction => (
                                transaction.AccountFromId == user.CurrentAccountId ||
                                transaction.AccountToId == user.CurrentAccountId))
                    .OrderByDescending(t => t.Date)
                    .ToList();


                ViewBag.Accounts = db.Accounts.ToList();
                ViewBag.AllTransactions = AllTransactions;
            }


            return View();
        }

        public IActionResult ViewAllTransactions()
        {
            return View();
        }

        public IActionResult AddTransaction()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddTransactionToAccount(string ToTitle, string Description, decimal Sum)
        {


            return RedirectToAction("index", "Transaction");
        }

        [HttpPost]
        public IActionResult AddTransactionFromAccount(string FromTitle, decimal Sum, string Description)
        {
            return RedirectToAction("index", "Transaction");
            return View();
        }

        [HttpPost]
        public IActionResult AddTransactionBetweenAccounts(string FromTitle, string ToTitle, decimal Sum, string Description)
        {

            if (FromTitle != "" || ToTitle != "")
            {
                return RedirectToAction("index", "Transaction");
            }


           



            return View();
        }
    }
}
