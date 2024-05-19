using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Security.Principal;
using WebApplication1.Models;
using WebApplication1.services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ApplicationDbContext context, UserManager<Person> userManager, ILogger<TransactionController> logger)
        {
            db = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} accessed the Index method of TransactionController.");

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


        public IActionResult MakeTransaction()
        {
            return View();
        }


        public async Task<IActionResult> SavaAsExcel()
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} requested to export transactions to Excel.");

            List<Transaction> AllTransactions = db.Transactions
                    .Where(
                        transaction => (
                                transaction.AccountFromId == user.CurrentAccountId ||
                                transaction.AccountToId == user.CurrentAccountId))
                    .OrderByDescending(t => t.Date)
                    .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Transactions");

                
                worksheet.Cells[1, 1].Value = "Transaction ID";
                worksheet.Cells[1, 2].Value = "Transaction Type";
                worksheet.Cells[1, 3].Value = "Account ID From";
                worksheet.Cells[1, 4].Value = "Account ID To";
                worksheet.Cells[1, 5].Value = "Description";
                worksheet.Cells[1, 6].Value = "Transaction Sum";
                worksheet.Cells[1, 7].Value = "Transaction Date";

              
                int row = 2;
                foreach (var transaction in AllTransactions)
                {
                    worksheet.Cells[row, 1].Value = transaction.TransactionId;
                    worksheet.Cells[row, 2].Value = transaction.Type;
                    worksheet.Cells[row, 3].Value = transaction.AccountFromId.HasValue ? transaction.AccountFromId.ToString() : "NULL";
                    worksheet.Cells[row, 4].Value = transaction.AccountToId.HasValue ? transaction.AccountToId.ToString() : "NULL";

                    worksheet.Cells[row, 5].Value = transaction.Description ?? "NULL";
                    worksheet.Cells[row, 6].Value = transaction.Sum.ToString();
                    worksheet.Cells[row, 7].Value = transaction.Date.ToString();
                    row++;
                }

                byte[] fileContents = excelPackage.GetAsByteArray();

                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Transactions.xlsx");
            }
        }



        [HttpPost]
        public async Task<IActionResult> AddTransaction_From_SomeWhere(string ToTitle, string Description, decimal Sum)
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} initiated transaction from somewhere.");

            var _AccountTo = db.Accounts.Where(account => account.Title == ToTitle).FirstOrDefault();

            if (_AccountTo != null)
            {
                

                var transaction = new Transaction()
                {
                    Type = 2,
                    Description = Description,
                    Sum = Sum,
                    Date = DateTime.UtcNow,
                    AccountFrom = null,
                    AccountTo = _AccountTo
                };

                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("index", "Transaction");
        }


        [HttpPost]
        public async Task<IActionResult> AddTransaction_To_SomeWhere(string FromTitle, string Description, decimal Sum)
        {
            var user = await _userManager.GetUserAsync(User);


            _logger.LogInformation($"User {user.UserName} initiated transaction to somewhere.");

            var _AccountFrom = db.Accounts.Where(account => account.Title == FromTitle).FirstOrDefault();

            if (_AccountFrom != null)
            {
                if (BalanceCounter.ClacBalance(_AccountFrom.AccountId, db) - Sum < 0)
                {
                    return RedirectToAction("index", "Transaction");
                }

                var transaction = new Transaction()
                {
                    Type = 1,
                    Description = Description,
                    Sum = Sum,
                    Date = DateTime.UtcNow,
                    AccountFrom = _AccountFrom,
                    AccountTo = null,

                };

                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
            }
            


            return RedirectToAction("index", "Transaction");
      
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction_Between_Accounts(
            string FromTitle, string ToTitle, string Description, decimal Sum)
        {

            if (FromTitle == ToTitle)
            {
                return RedirectToAction("index", "Transaction");
            }


            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} initiated transaction between accounts.");

            var _AccountFrom = db.Accounts.Where(account => account.Title == FromTitle).FirstOrDefault();
            var _AccountTo = db.Accounts.Where(account => account.Title == ToTitle).FirstOrDefault();

            if (_AccountFrom != null && _AccountTo != null )
            {

                if (BalanceCounter.ClacBalance(_AccountFrom.AccountId, db) - Sum < 0)
                {
                    return RedirectToAction("index", "Transaction");
                }


                var transaction = new Transaction()
                {
                    Type = 3,
                    Description = Description,
                    Sum = Sum,
                    Date = DateTime.UtcNow,
                    AccountFrom = _AccountFrom,
                    AccountTo = _AccountTo,

                };


                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();
                
            }


            return RedirectToAction("index", "Transaction");

        }



    }
}
