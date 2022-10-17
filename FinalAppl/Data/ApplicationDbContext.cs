using CakeApplication.Model;
using Microsoft.EntityFrameworkCore;

namespace CakeApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> users { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Item> items { get; set; }
    }
}
