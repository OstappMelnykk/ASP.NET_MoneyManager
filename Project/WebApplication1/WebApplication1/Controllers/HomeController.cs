using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public HomeController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {

            ViewBag.Name = User.Identity.Name;
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;

            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View();

            }
            var email = user.Email;
            var userId = user.Id;


            ViewBag.Email = email;
            ViewBag.UserId = userId;


            


            bool HasAccount = user.CurrentAccountId != null;

            ViewBag.HasAccount = HasAccount;




            ViewBag.LastTransactions = new List<int>();
            ViewBag.AccountName = "Sdfsdsd";

            if (HasAccount)
            {

                ViewBag.AccountName = db
                    .Accounts
                    .Where(account => account.AccountId == user.CurrentAccountId).FirstOrDefault().Title;

                DateTime dateMin = new DateTime(2023, 5, 14).ToUniversalTime();
                DateTime dateMax = new DateTime(2025, 5, 10).ToUniversalTime();



                List<Transaction> lastTransactionsFrom = db.Transactions.Where(transaction => (transaction.AccountFromId == user.CurrentAccountId
                || transaction.AccountToId == user.CurrentAccountId) && transaction.Date <= dateMax &&
                transaction.Date >= dateMin).OrderByDescending(t => t.Date).ToList();



                ViewBag.LastTransactions = lastTransactionsFrom;


            }





            return View();
        }

        public IActionResult AccessDenied() => View();
    }
}
