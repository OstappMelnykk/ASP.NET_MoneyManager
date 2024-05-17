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
    public class GoalController : Controller
    {
        ApplicationDbContext db;
        private readonly UserManager<Person> _userManager;
        public GoalController(ApplicationDbContext context, UserManager<Person> userManager)
        {
            db = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            List<Goal> AccountGoals = db.Goals.Where(goal => goal.AccountId == user.CurrentAccountId).ToList();

            ViewBag.AccountGoals = AccountGoals;
            ViewBag.DbContext = db;


            return View();
        }


        public async Task<IActionResult> AddGoal(string Title, decimal AmountToCollect, string Description)
        {
            var user = await _userManager.GetUserAsync(User);

            if (AmountToCollect <= 0)
            {
                return RedirectToAction("Index", "Goal");
            }


            var _Account = db.Accounts.Where(account => account.AccountId == user.CurrentAccountId).FirstOrDefault();

            if (_Account != null)
            {


                Goal newGoal = new Goal
                {
                    Title = Title,
                    Description = Description,
                    AmountToCollect = AmountToCollect,
                    Account = _Account
                };

                db.Goals.Add(newGoal);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Goal");
        }

        public async Task<IActionResult> GoalDetails()
        {
            return View();
        }
        
        
        public async Task<IActionResult> GoalDelete(int Id)
        {
            var user = await _userManager.GetUserAsync(User);

            var wantedGoal = db.Goals.Where(goal => goal.GoalId == Id).FirstOrDefault();
                

            if (wantedGoal == null)
                return RedirectToAction("Index", "Goal");


            

            db.Goals.Remove(wantedGoal);
            await db.SaveChangesAsync();


            return RedirectToAction("Index", "Goal");


            
        }

    }
}
