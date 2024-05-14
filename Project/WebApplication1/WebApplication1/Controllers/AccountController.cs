using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;


namespace WebApplication1.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        ApplicationDbContext db;


        private readonly UserManager<Person> _userManager;
        private readonly SignInManager<Person> _signInManager;

        public AccountController(ApplicationDbContext context, UserManager<Person> userManager, SignInManager<Person> signInManager)
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already in use.");
                    return View(model);
                }

                Person user = new Person
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    PhoneNumber = model.PhoneNumber,
                    Photo = null,
                    Accounts = new List<Account>()
                };

    
                Account account1 = new Account()
                {
                    Title = "account1",
                    Person = user,
                    Goals = new List<Goal>(),
                    TransactionsOnTheAccount = new List<Transaction>(),
                    TransactionsFromTheAccount = new List<Transaction>(),
                };

                Account account2 = new Account()
                {
                    Title = "account2",
                    Person = user,
                    Goals = new List<Goal>(),
                    TransactionsOnTheAccount = new List<Transaction>(),
                    TransactionsFromTheAccount = new List<Transaction>(),
                };


                user.Accounts.Add(account1);
                user.Accounts.Add(account2);


                Transaction transaction1 = new Transaction()
                {
                    Type = 2,
                    Description = "Description transaction1",
                    Sum = 100m,
                    Date = new DateTime(2024, 5, 3).ToUniversalTime(),
                    AccountFrom = account2,
                    AccountTo = account1,
                };

                account1.TransactionsOnTheAccount.Add(transaction1);
                //db.Transactions.Add(transaction1);

                //account2.TransactionsFromTheAccount.Add(transaction1);



                /*
                Transaction transaction2 = new Transaction()
                {
                    Type = 1,
                    Description = "desk,",
                    Sum = 50m,
                    Date = new DateTime(2024, 5, 14).ToUniversalTime(), 
                    AccountFrom = account1,
                    AccountTo = account2,
                };

                account1.TransactionsFromTheAccount.Add(transaction2);
                account2.TransactionsOnTheAccount.Add(transaction2);


                Transaction transaction3 = new Transaction()
                {
                    Type = 3,
                    Description = "desk,",
                    Sum = 20m,
                    Date = new DateTime(2024, 5, 14).ToUniversalTime(),
                    AccountFrom = account1,
                    AccountTo = null,
                };

                account1.TransactionsFromTheAccount.Add(transaction3);

                Transaction transaction4 = new Transaction()
                {
                    Type = 3,
                    Description = "desk,",
                    Sum = 30m,
                    Date = new DateTime(2024, 5, 14).ToUniversalTime(),
                    AccountFrom = null,
                    AccountTo = account2,
                };

                account2.TransactionsOnTheAccount.Add(transaction4);*/


                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (user.UserName == "ADMIN")
                    {
                        _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
                    }
                    else
                    {
                        _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User")).GetAwaiter().GetResult();
                    }

                    await _signInManager.SignInAsync(user, false);
   

                    user.CurrentAccount = account1;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }





        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            return Redirect(model.ReturnUrl);
                        else
                            return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Incorrect login and (or) password");
            }

            return View(model);
        }




        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Home/Index");
        }
    }
}
