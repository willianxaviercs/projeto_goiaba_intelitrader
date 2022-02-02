using System;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using UserApi.Models;
using UserApi.Controllers;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserApi.Tests
{
    public class UsersControllerTests
    {
        public ServiceProvider serviceProvider;
        public ILoggerFactory factory;
        public ILogger<UsersController> logger;
        public UsersController Controller;
        public User ValidDummyUser;

        public UsersControllerTests()
        {
            // controller logger
            serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();

            factory = serviceProvider.GetService<ILoggerFactory>();

            logger = factory.CreateLogger<UsersController>();

            // in-memory database
            var options = new DbContextOptionsBuilder<UserDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDbContext")
                .Options;

            var DbContext = new UserDbContext(options);

            Controller = new UsersController(logger, DbContext);
            
            // 
            ValidDummyUser = new User
            {
                FirstName = "Bill",
                SurName = "Gates",
                Age = 66
            };

            DbContext.Add(ValidDummyUser);
            DbContext.SaveChanges();
        }

        // http response -> created (201)
        // value -> entity created
        [Fact]
        public void CreateUser_Success()
        {
            var UserToCreate = new User
            {
                FirstName = "Linus",
                SurName = "Torvalds",
                Age = 52
            };

            var ActionResult = Controller.CreateUser(UserToCreate);

            var Result = ActionResult.Result as CreatedAtActionResult;

            Assert.IsType<CreatedAtActionResult>(Result);

            var ResultValue = Result.Value as User;

            Assert.IsType<User>(ResultValue);

            Assert.True(ResultValue.Id == UserToCreate.Id);
        }

        // http response -> ok (200)
        // value -> entity found
        [Fact]
        public void GetUsersById_Success()
        {
            var ActionResult = Controller.GetUserById(ValidDummyUser.Id);

            var Result = ActionResult.Result as OkObjectResult;

            var ResultValue = Result.Value as User;

            Assert.IsType<OkObjectResult>(Result);
            Assert.IsType<User>(ResultValue);
            Assert.Equal(ResultValue.Id,  ValidDummyUser.Id);
        }

        // http response -> not found (404)
        [Fact]
        public void GetUsersById_NotFound()
        {
            Guid InvalidId = new Guid("00000000-0000-0000-0000-000000000000");

            var ActionResult = Controller.GetUserById(InvalidId);

            var Result = ActionResult.Result as NotFoundResult;

            Assert.IsType<NotFoundResult>(Result);
        }

        // http response -> no content (204)
        [Fact]
        public void UpdateUser_Success()
        {
            User UpdatedUser = new User 
            {
                FirstName = "Steve",
                SurName = "Jobs",
                Age = 56
            };

            var ActionResult = Controller.UpdateUser(ValidDummyUser.Id, UpdatedUser);

            Assert.IsType<NoContentResult>(ActionResult);
        }

        // http response -> bad request (400)
        [Fact]
        public void UpdateUser_Fail()
        {
            Guid InvalidId = new Guid("00000000-0000-0000-0000-000000000000");

            User UpdatedUser = new User 
            {
                FirstName = "Steve",
                SurName = "Jobs",
                Age = 56
            };

            var UpdateResult = Controller.UpdateUser(InvalidId, UpdatedUser);

            Assert.IsType<BadRequestResult>(UpdateResult);
        }

        // http response -> ok (200)
        // value -> entity deleted
        [Fact]
        public void DeleteUser_Success()
        {
            var ActionResult = Controller.DeleteUser(ValidDummyUser.Id);

            Assert.IsType<OkObjectResult>(ActionResult.Result);

            var StatusResult = ActionResult.Result as OkObjectResult;
            var UserResult = StatusResult.Value as User; 

            Assert.IsType<User>(UserResult);
            Assert.Equal(UserResult.Id, ValidDummyUser.Id);
        }

        // http response -> not found (404)
        [Fact]
        public void DeleteUser_NotFound()
        {
            Guid InvalidId = new Guid("00000000-0000-0000-0000-000000000000");
            var ActionResult = Controller.DeleteUser(InvalidId);

            Assert.IsType<NotFoundResult>(ActionResult.Result);
        }
    }
}
