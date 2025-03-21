using MessMeApi.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace MessMeApi.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
