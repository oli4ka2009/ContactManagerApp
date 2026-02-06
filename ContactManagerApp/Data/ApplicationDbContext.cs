using ContactManagerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Manager> Managers { get; set; }
    }
}
