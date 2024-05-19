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
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            ApplicationDbContext context, 
            UserManager<Person> userManager, 
            SignInManager<Person> signInManager,
            ILogger<AccountController> logger
            )
        {
            db = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();



        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            _logger.LogInformation("Register action called with email: {email}, username: {username}, and phone number: {phoneNumber}", model.Email, model.UserName, model.PhoneNumber);
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    _logger.LogError("Registration failed. Email {email} is already in use.", model.Email);
                    ModelState.AddModelError(string.Empty, "This email is already in use.");
                    return View(model);
                }
                _logger.LogInformation("Creating new user with email: {email}, username: {username}, and phone number: {phoneNumber}", model.Email, model.UserName, model.PhoneNumber);

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
                    Title = "DefaultAccount",
                    Person = user,
                    Goals = new List<Goal>(),
                    TransactionsOnTheAccount = new List<Transaction>(),
                    TransactionsFromTheAccount = new List<Transaction>(),
                };



                db.Accounts.Add(account1);

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (user.UserName == "ADMIN")
                    {
                        _logger.LogInformation($"New user is registering. User get Administrator role");

                       _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator")).GetAwaiter().GetResult();
                    }
                    else
                    {
                        _logger.LogInformation($"New user is registering. User get User role");
                        _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User")).GetAwaiter().GetResult();
                    }

                    await _signInManager.SignInAsync(user, false);
   

                    user.CurrentAccount = account1;
                    await db.SaveChangesAsync();


                    _logger.LogInformation("User registered successfully with email: {email}", model.Email);
                    return RedirectToAction("Index", "Default");
                }
                else
                {
                    
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError("Registration failed with error: {error}", error.Description);
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                   
                }
            }
            _logger.LogError("Registration failed due to invalid model state.");
            return View(model);
        }





        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            _logger.LogInformation("Login page accessed.");
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {

            _logger.LogInformation("Login attempt with email: {email}", model.Email);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, false);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User logged in successfully with email: {email}", model.Email);
                        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                            return Redirect(model.ReturnUrl);
                        else
                            return RedirectToAction("Index", "Default");

                    }
                   
                }
                _logger.LogError("Login failed for email: {email}", model.Email);
                ModelState.AddModelError("", "Incorrect login and (or) password");
            }



            return View(model);
        }





        public IActionResult ChangePassword()
        {
            _logger.LogInformation("Change password page accessed.");
            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            _logger.LogInformation("Change password attempt.");
            if (ModelState.IsValid)
            {
                
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    _logger.LogError("User not found during change password attempt.");
                    return RedirectToAction("Login", "Account");
                }
         

                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                
                if (!result.Succeeded)
                {
                    _logger.LogError("Change password failed.");
                    foreach (var error in result.Errors)
                    {
                        
                      
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    
                    return View();
                }

                _logger.LogInformation("Password changed successfully.");
                await _signInManager.RefreshSignInAsync(user);

                return RedirectToAction("ChangePasswordConfirmation", "Account");
            }
            _logger.LogError("Change password attempt failed due to invalid model state.");
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePasswordConfirmation()
        {
            _logger.LogInformation("Change password confirmation page accessed.");
            return View();
        }



        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User logged out.");
            await _signInManager.SignOutAsync();
            return Redirect("/Default/Index");
        }
    }
}
