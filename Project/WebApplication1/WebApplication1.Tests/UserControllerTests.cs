using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        private Mock<UserManager<Person>> _userManagerMock;
        private ApplicationDbContext _dbContext;
        private UserController _controller;

        [TestInitialize]
        public void Setup()
        {

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_" + System.Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDbContext(options);

            _userManagerMock = new Mock<UserManager<Person>>(
                Mock.Of<IUserStore<Person>>(), null, null, null, null, null, null, null, null
            );

            _controller = new UserController(_dbContext, _userManagerMock.Object);


            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user-id"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task UserMainPage_ReturnsViewWithUserDetails()
        {
   
            var user = new Person
            {
                Id = "user-id",
                UserName = "testuser",
                Email = "testuser@example.com",
                PhoneNumber = "123-456-7890"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

       
            var result = await _controller.UserMainPage() as ViewResult;

    
            Assert.IsNotNull(result);
            Assert.AreEqual("123-456-7890", _controller.ViewBag.phoneNumber);
            Assert.AreEqual("testuser@example.com", _controller.ViewBag.email);
            Assert.AreEqual("testuser", _controller.ViewBag.userName);
        }

        [TestMethod]
        public async Task Settings_UpdatesUserDetails()
        {

            var user = new Person
            {
                Id = "user-id",
                UserName = "testuser",
                Email = "testuser@example.com",
                PhoneNumber = "123-456-7890"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

  
            var result = await _controller.Settings("newuser", "987-654-3210", "newuser@example.com") as ViewResult;

   
            Assert.IsNotNull(result);
            Assert.AreEqual("newuser", user.UserName);
            Assert.AreEqual("987-654-3210", user.PhoneNumber);
            Assert.AreEqual("newuser@example.com", user.Email);
        }
    }
}
