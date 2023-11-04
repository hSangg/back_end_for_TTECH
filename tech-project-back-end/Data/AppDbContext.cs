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
            modelBuilder.Entity<Product>().HasKey(c => c.product_id);
            modelBuilder.Entity<Image>().HasKey(c => c.image_id);
            modelBuilder.Entity<Cart>().HasKey(c => new { c.user_id, c.product_id });
            modelBuilder.Entity<Product_Category>()
        .HasKey(pc => new { pc.ProductId, pc.CategoryId });

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
        public DbSet<Product_Category> Product_Category { get; set; }

        public DbSet<Cart> Cart{ get; set; }


    }
}
