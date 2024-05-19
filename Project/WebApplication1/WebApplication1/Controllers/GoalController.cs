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
       
        private readonly ILogger<GoalController> _logger;

        public GoalController(ApplicationDbContext context, UserManager<Person> userManager, ILogger<GoalController> logger)
        {
            db = context;
            _userManager = userManager;
            _logger = logger;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} accessed the Index method of GoalController.");

            List<Goal> AccountGoals = db.Goals.Where(goal => goal.AccountId == user.CurrentAccountId).ToList();

            ViewBag.AccountGoals = AccountGoals;
            ViewBag.DbContext = db;


            return View();
        }


        public async Task<IActionResult> AddGoal(string Title, decimal AmountToCollect, string Description)
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} is adding a new goal.");

            if (AmountToCollect <= 0)
            {
                _logger.LogWarning($"User {user.UserName} attempted to add a goal with invalid amount.");
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
            _logger.LogInformation("Goal details method accessed.");
            return View();
        }
        
        
        public async Task<IActionResult> GoalDelete(int Id)
        {
            var user = await _userManager.GetUserAsync(User);

            _logger.LogInformation($"User {user.UserName} is attempting to delete goal with ID: {Id}.");

            var wantedGoal = db.Goals.Where(goal => goal.GoalId == Id).FirstOrDefault();
                

            if (wantedGoal == null)

                _logger.LogWarning($"User {user.UserName} attempted to delete a non-existing goal.");
            return RedirectToAction("Index", "Goal");


            

            db.Goals.Remove(wantedGoal);
            await db.SaveChangesAsync();


            return RedirectToAction("Index", "Goal");


            
        }

    }
}
