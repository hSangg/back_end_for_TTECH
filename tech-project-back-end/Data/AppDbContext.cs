using Microsoft.EntityFrameworkCore;
using System.Xml;
using tech_project_back_end.Models;

namespace tech_project_back_end.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(c => c.category_id);
            modelBuilder.Entity<User>().HasKey(c => c.user_id);
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<User> User { get; set; }
    }
}
