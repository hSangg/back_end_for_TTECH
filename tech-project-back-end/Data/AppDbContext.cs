﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;
using System.Text;
using tech_project_back_end.Models;

namespace tech_project_back_end.Data
{
    public class AppDbContext : DbContext
    {
        public readonly IHttpContextAccessor _httpContextAccessor;
        public AppDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<User>().HasKey(c => c.UserId);
            modelBuilder.Entity<Supplier>().HasKey(c => c.SupplierId);
            modelBuilder.Entity<Product>().HasKey(c => c.ProductId);
            modelBuilder.Entity<Order>().HasKey(c => c.OrderId);
            modelBuilder.Entity<Image>().HasKey(c => c.ImageId);
            modelBuilder.Entity<Cart>().HasKey(c => new { c.user_id, c.product_id });
            modelBuilder.Entity<DetailOrder>().HasKey(c => new { c.OrderId, c.ProductId });
            modelBuilder.Entity<Discount>().HasKey(c => c.DiscountId);

            modelBuilder.Entity<Supplier>()
                .Property(s => s.SupplierId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<DetailOrder>()
                .HasOne(d => d.Order)
                .WithMany(o => o.DetailOrders)  
                .HasForeignKey(d => d.OrderId)
                .HasPrincipalKey(o => o.OrderId);

            modelBuilder.Entity<DetailOrder>()
                .HasOne(d => d.Product)
                .WithMany(p => p.DetailOrders)  
                .HasForeignKey(d => d.ProductId)
                .HasPrincipalKey(p => p.ProductId);

            modelBuilder.Entity<Image>()
                .HasOne(i => i.Product)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.ProductId)
                .HasPrincipalKey(p => p.ProductId);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                || e.State == EntityState.Modified
                || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entity in modifiedEntities)
            {
                var aditLog = new AuditLogs
                {
                    EntityName = entity.Entity.GetType().Name,
                    UserName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name) ?? "DefaultName",
                    UserId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "DefaultId",
                    Role = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role) ?? "default role",
                    Action = entity.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Changes = GetChanges(entity),
                };

                AuditLogs.Add(aditLog);
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        private string GetChanges(EntityEntry entity)
        {
            var changes = new StringBuilder();

            foreach (var property in entity.OriginalValues.Properties)
            {
                var originalValue = entity.OriginalValues[property];
                var currentValue = entity.CurrentValues[property];

                if (!Equals(originalValue, currentValue))
                {
                    changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
                }
            }

            return changes.ToString();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
        }

        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Image> Image { get; set; }
        public DbSet<DetailOrder> DetailOrder { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<AuditLogs> AuditLogs { get; set; }
    }
}
