using Microsoft.EntityFrameworkCore;
using UserApi.Models;

namespace UserApi.Models
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> UserItems { get; set; }

        // Constructor
        public UserDbContext(DbContextOptions<UserDbContext> options) :base(options)
        {

        }
    }
}
