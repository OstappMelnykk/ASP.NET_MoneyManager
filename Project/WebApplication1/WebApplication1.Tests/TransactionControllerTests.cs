using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.services;

namespace WebApplication1.Tests
{
    [TestClass]
    public class TransactionControllerTests
    {
        private TransactionController _controller;
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


            var userId = "1"; 
            mockUserManager.Setup(m => m.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(new Person { CurrentAccountId = 1, Id = userId });

            _userManager = mockUserManager.Object;


            _controller = new TransactionController(_context, _userManager);


            _context.Users.Add(new Person { Email = "dasda@casda", UserName = "dsada", Id = "1", PhoneNumber = "33333333" });
            _context.Accounts.Add(new Account { AccountId = 1, Title = "Test Account", PersonId = userId });
            _context.Accounts.Add(new Account { AccountId = 2, Title = "Test Account 2", PersonId = userId }); 

            _context.SaveChanges();
        }





        [TestMethod]
        public async Task SavaAsExcel_ReturnsFileResult()
        {
 
            _context.Transactions.Add(new Transaction { TransactionId = 1, Type = 1, Description = "Test Transaction", Sum = 100, Date = DateTime.Now, AccountFromId = 1 });
            _context.SaveChanges();

  
            var result = await _controller.SavaAsExcel() as FileResult;

       
            Assert.IsNotNull(result);
            Assert.AreEqual("Transactions.xlsx", result.FileDownloadName);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", result.ContentType);
        }

        [TestMethod]
        public async Task AddTransaction_From_SomeWhere_AddsTransaction()
        {
 
            var result = await _controller.AddTransaction_From_SomeWhere("Test Account", "Test Description", 100) as RedirectToActionResult;


            Assert.IsNotNull(result);
            Assert.AreEqual("index", result.ActionName);
            var addedTransaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Description == "Test Description");
            Assert.IsNotNull(addedTransaction);
        }

        

        [TestMethod]
        public async Task AddTransaction_Between_Accounts_Redirects_If_Source_Destination_Are_Same()
        {
           
            var result = await _controller.AddTransaction_Between_Accounts("Test Account", "Test Account", "Test Description", 100) as RedirectToActionResult;

       
            Assert.IsNotNull(result);
            Assert.AreEqual("index", result.ActionName);


            var transactionCount = await _context.Transactions.CountAsync();
            Assert.AreEqual(0, transactionCount);
        }

        [TestMethod]
        public async Task AddTransaction_Between_Accounts_Redirects_If_Insufficient_Balance()
        {
   



            var result = await _controller.AddTransaction_Between_Accounts("Test Account", "Test Account 2", "Test Description", 1000) as RedirectToActionResult;

    
            Assert.IsNotNull(result);
            Assert.AreEqual("index", result.ActionName);

     
            var transactionCount = await _context.Transactions.CountAsync();
            Assert.AreEqual(0, transactionCount);
        }

        
        [TestMethod]
        public async Task AddTransaction_To_SomeWhere_Redirects_If_Insufficient_Balance()
        {
            
            var fromTitle = "Test Account";
            var description = "Test Description";
            var sum = 1000; 

            
            var result = await _controller.AddTransaction_To_SomeWhere(fromTitle, description, sum) as RedirectToActionResult;

           
            Assert.IsNotNull(result);
            Assert.AreEqual("index", result.ActionName);

    
            var transactionCount = await _context.Transactions.CountAsync();
            Assert.AreEqual(0, transactionCount);
        }


    }
}
