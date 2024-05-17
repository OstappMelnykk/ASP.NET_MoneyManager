using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;
using WebApplication1.Models;
using WebApplication1.services;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class WalletController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public WalletController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            List<Account> accounts = db.Accounts.Where(account => account.PersonId == user.Id).ToList();
           
            ViewBag.DbContext = db;
            ViewBag.Accounts = accounts;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddWallet(string Title)
        {
            var user = await _userManager.GetUserAsync(User);

            if (Title == "" || Title == null)
            {
                return RedirectToAction("Index", "Wallet");
            }
            foreach (var item in db.Accounts)
            {
                if (item.Title == Title)
                {
                    return RedirectToAction("Index", "Wallet");
                }
            }

           
            
            Account account = new Account()
            {
                Title = Title,
                Person = user,
                Goals = new List<Goal>(),
                TransactionsOnTheAccount = new List<Transaction>(),
                TransactionsFromTheAccount = new List<Transaction>(),
            };

            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Wallet");
        }

        public async Task<IActionResult> ChooseWallet(string AccountTitle)
        {
            var user = await _userManager.GetUserAsync(User);


            user.CurrentAccount = db.Accounts.Where(account => account.PersonId == user.Id)
                .Where(account => account.Title == AccountTitle)
                .FirstOrDefault();

            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }





        public async Task<IActionResult> DeleteWallet(string AccountTitle)
        {
            var user = await _userManager.GetUserAsync(User);

            var wantedAccount = db.Accounts
                .Include(a => a.TransactionsOnTheAccount)
                .Include(a => a.TransactionsFromTheAccount)
                .SingleOrDefault(account => account.PersonId == user.Id && account.Title == AccountTitle);

            if (wantedAccount == null)
                return RedirectToAction("Index", "Wallet");

            
            db.Transactions.RemoveRange(wantedAccount.TransactionsOnTheAccount);
            db.Transactions.RemoveRange(wantedAccount.TransactionsFromTheAccount);

            db.Accounts.Remove(wantedAccount);
            await db.SaveChangesAsync();

            
            var remainingAccountsCount = await db.Accounts.CountAsync(account => account.PersonId == user.Id);
            if (user.CurrentAccountId == wantedAccount.AccountId && remainingAccountsCount == 0)
            {
                
                var newAccount = new Account
                {
                    Title = "Default",
                    Person = user,
                    Goals = new List<Goal>(),
                    TransactionsOnTheAccount = new List<Transaction>(),
                    TransactionsFromTheAccount = new List<Transaction>()
                };

                db.Accounts.Add(newAccount);
                await db.SaveChangesAsync();

                user.CurrentAccount = newAccount;
                await db.SaveChangesAsync();
            }
            else if (user.CurrentAccountId == wantedAccount.AccountId)
            {
                
                user.CurrentAccount = await db.Accounts.FirstOrDefaultAsync(account => account.PersonId == user.Id);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Wallet");
        }





        public IActionResult Statistic()
        {
            return View();
        }
    }
}
