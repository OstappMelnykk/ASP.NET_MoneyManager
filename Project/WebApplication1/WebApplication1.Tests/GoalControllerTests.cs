using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.services;

namespace WebApplication1.Tests
{
    [TestClass]
    public class GoalControllerTests
    {
        private GoalController _controller;
        private ApplicationDbContext _context;
        private UserManager<Person> _userManager;

        [TestInitialize]
        public void Setup()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_Database")
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new ApplicationDbContext(options);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "1"),
        new Claim(ClaimTypes.Name, "testuser")
            }));

            var mockUserManager = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null);
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Person { CurrentAccountId = 1 });

            _userManager = mockUserManager.Object;


            _controller = new GoalController(_context, _userManager, null);
        }
     


        [TestMethod]
        public async Task AddGoal_WithValidData_RedirectsToIndex()
        {
  
            var result = await _controller.AddGoal("New Goal", 200, "Description") as RedirectToActionResult;

  
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestMethod]
        public async Task GoalDelete_WithValidId_RedirectsToIndex()
        {
            var testGoal = new Goal { GoalId = 1, Title = "Test Goal", AmountToCollect = 100, Description = "Test Description", AccountId = 1 };
            _context.Goals.Add(testGoal);
            _context.SaveChanges();


            var result = await _controller.GoalDelete(1) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
        }
    }
}
