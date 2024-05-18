using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Models.ViewModels;

namespace WebApplication1.Tests
{
    [TestClass]
    public class AccountControllerTests
    {
        private Mock<UserManager<Person>> _userManagerMock;
        private Mock<SignInManager<Person>> _signInManagerMock;
        private ApplicationDbContext _dbContext;
        private AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            _userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null
            );

            _signInManagerMock = new Mock<SignInManager<Person>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<Person>>(),
                null, null, null, null
            );

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _controller = new AccountController(_dbContext, _userManagerMock.Object, _signInManagerMock.Object);
        }

        [TestMethod]
        public async Task Register_WithValidModel_ShouldCreateUser()
        {
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                UserName = "testuser",
                PhoneNumber = "1234567890",
                Password = "Password123!",
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((Person)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<Person>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(sm => sm.SignInAsync(It.IsAny<Person>(), It.IsAny<bool>(), null)).Returns(Task.CompletedTask);

            var result = await _controller.Register(model) as RedirectToActionResult;

      
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Default", result.ControllerName);
        }

        [TestMethod]
        public async Task Register_WithExistingEmail_ShouldReturnError()
        {
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                UserName = "testuser",
                PhoneNumber = "1234567890",
                Password = "Password123!",
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person());

            var result = await _controller.Register(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.AreEqual("This email is already in use.", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public async Task Login_WithValidCredentials_ShouldRedirect()
        {
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "Password123!",
                RememberMe = false
            };

            var user = new Person { UserName = "testuser" };
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            var result = await _controller.Login(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Default", result.ControllerName);
        }

        [TestMethod]
        public async Task Login_WithInvalidCredentials_ShouldReturnError()
        {
            var model = new LoginViewModel
            {
                Email = "test@example.com",
                Password = "WrongPassword!",
                RememberMe = false
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new Person());
            _signInManagerMock.Setup(sm => sm.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(model) as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(model, result.Model);
            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.AreEqual("Incorrect login and (or) password", _controller.ModelState[string.Empty].Errors[0].ErrorMessage);
        }

        [TestMethod]
        public async Task ChangePassword_WithValidData_ShouldSucceed()
        {
            var model = new ChangePasswordViewModel
            {
                OldPassword = "OldPassword123!",
                NewPassword = "NewPassword123!",
                ConfirmPassword = "NewPassword123!"
            };

            var user = new Person { UserName = "testuser" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.ChangePasswordAsync(It.IsAny<Person>(), It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);
            _signInManagerMock.Setup(sm => sm.RefreshSignInAsync(It.IsAny<Person>())).Returns(Task.CompletedTask);

            var result = await _controller.ChangePassword(model) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("ChangePasswordConfirmation", result.ActionName);
            Assert.AreEqual("Account", result.ControllerName);
        }

        [TestMethod]
        public async Task Logout_ShouldRedirectToIndex()
        {
            _signInManagerMock.Setup(sm => sm.SignOutAsync()).Returns(Task.CompletedTask);

           
            var result = await _controller.Logout() as RedirectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("/Default/Index", result.Url);
        }
    }
}
