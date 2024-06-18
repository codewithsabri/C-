using Microsoft.EntityFrameworkCore;
using userProject.Models;

namespace userProject.Data
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<UserViewModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserViewModel>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}