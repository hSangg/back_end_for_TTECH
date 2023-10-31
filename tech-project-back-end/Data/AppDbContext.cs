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
            modelBuilder.Entity<Supplier>().HasKey(c => c.supplier_id);
            modelBuilder.Entity<Product>().HasKey(c => c.ProductId);
            modelBuilder.Entity<Image>().HasKey(c => c.image_id);
        }


       protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Other configurations...
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        }

        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Image> Image { get; set; }

    }
}
