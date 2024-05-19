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
        private readonly ILogger<WalletController> _logger;
        public WalletController(ApplicationDbContext context, UserManager<Person> userManager, ILogger<WalletController> logger)
        {
            db = context;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} accessed the Index method of WalletController.");

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
                _logger.LogWarning($"User {user.UserName} attempted to add a wallet with an empty title.");
                return RedirectToAction("Index", "Wallet");
            }
            foreach (var item in db.Accounts)
            {
                if (item.Title == Title)
                {
                    _logger.LogWarning($"User {user.UserName} attempted to add a wallet with a title that already exists: {Title}.");
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

            _logger.LogInformation($"User {user.UserName} added a new wallet with title: {Title}.");

            return RedirectToAction("Index", "Wallet");
        }

        public async Task<IActionResult> ChooseWallet(string AccountTitle)
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} chose the wallet with title: {AccountTitle}.");

            user.CurrentAccount = db.Accounts.Where(account => account.PersonId == user.Id)
                .Where(account => account.Title == AccountTitle)
                .FirstOrDefault();

            await db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }





        public async Task<IActionResult> DeleteWallet(string AccountTitle)
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} requested to delete the wallet with title: {AccountTitle}.");

            var wantedAccount = db.Accounts
                .Include(a => a.TransactionsOnTheAccount)
                .Include(a => a.TransactionsFromTheAccount)
                .SingleOrDefault(account => account.PersonId == user.Id && account.Title == AccountTitle);

            if (wantedAccount == null)
            {
                _logger.LogWarning($"User {user.UserName} attempted to delete a non-existing wallet with title: {AccountTitle}.");
                return RedirectToAction("Index", "Wallet");
            }
                

            
            db.Transactions.RemoveRange(wantedAccount.TransactionsOnTheAccount);
            db.Transactions.RemoveRange(wantedAccount.TransactionsFromTheAccount);

            db.Accounts.Remove(wantedAccount);
            await db.SaveChangesAsync();

            _logger.LogInformation($"User {user.UserName} deleted the wallet with title: {AccountTitle}.");

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
