using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using UserApi.Models;

namespace UserApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        public readonly UserDbContext _context; // Db Storage
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger, UserDbContext dbcontext)
        {
            _logger = logger;
            _context = dbcontext;
        }

        // GET: /users      -> Returns an array of all users
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            return Ok(_context.UserItems);
        }

        // GET: /users/id
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(Guid id)
        {
            var UserFound = _context.UserItems.Find(id);

            if (UserFound == null)
            {
                return NotFound();
            }

            return Ok(UserFound);
        }

        // POST: /users    -> Creates an new user
        [HttpPost]
        public ActionResult<User> CreateUser(User user)
        {
            _context.UserItems.Add(user);
            _context.SaveChanges();

            // log
            string LogInfo = $"User {user.Id} created at {user.CreationTime}";
            _logger.LogInformation(LogInfo);

            return CreatedAtAction(nameof(GetUserById), user, user);
        }

        // PUT: /users/id -> Updates an existing user
        [HttpPut("{id}")]
        public ActionResult UpdateUser(Guid id , User user)
        {
            // check if user exists
            User UserToUpdate = _context.UserItems.Find(id);

            if (UserToUpdate == null)
            {
                return BadRequest();
            }

            UserToUpdate.FirstName = user.FirstName;
            UserToUpdate.SurName = user.SurName;
            UserToUpdate.Age = user.Age;

            _context.Entry(UserToUpdate).State = EntityState.Modified;
            _context.SaveChanges();

            string LogInfo = $"User {UserToUpdate.Id} updated!";
            _logger.LogInformation(LogInfo);

            return NoContent();
        }

        // DELETE: /users/id -> Deletes and existing user
        [HttpDelete("{id}")]
        public ActionResult<User> DeleteUser(Guid id)
        {
            var UserToDelete = _context.UserItems.Find(id);

            if (UserToDelete == null)
            {
                return NotFound();
            }

            _context.UserItems.Remove(UserToDelete);
            _context.SaveChanges();

            string LogInfo = $"User {UserToDelete.Id} deleted!";
            _logger.LogInformation(LogInfo);

            return Ok(UserToDelete);
        }
    }
}
