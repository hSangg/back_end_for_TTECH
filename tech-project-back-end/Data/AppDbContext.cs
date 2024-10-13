using Microsoft.EntityFrameworkCore;
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

        public void SeedData()
        {
            if (!Category.Any())
            {
                var categories = new List<Category>
                {
                    new() { CategoryId = "COMP001", CategoryName = "Computers & Laptops" },
                    new() { CategoryId = "MOBIL002", CategoryName = "Mobile Phones & Tablets" },
                    new() { CategoryId = "AUDIO003", CategoryName = "Audio & Headphones" },
                    new() { CategoryId = "GAME004", CategoryName = "Gaming & Consoles" },
                    new() { CategoryId = "SMART005", CategoryName = "Smart Home Devices" },
                    new() { CategoryId = "WEARB006", CategoryName = "Wearable Technology" },
                    new() { CategoryId = "CAMERA007", CategoryName = "Cameras & Photography" },
                    new() { CategoryId = "TV008", CategoryName = "TVs & Home Theater" },
                    new() { CategoryId = "NETW009", CategoryName = "Networking & Wireless" },
                    new() { CategoryId = "COMPA010", CategoryName = "Computer Components" },
                    new() { CategoryId = "PRINT011", CategoryName = "Printers & Scanners" },
                    new() { CategoryId = "STORA012", CategoryName = "Storage & Drives" },
                    new() { CategoryId = "ACCES013", CategoryName = "Accessories & Peripherals" },
                    new() { CategoryId = "SOFTW014", CategoryName = "Software & Services" },
                    new() { CategoryId = "OFFIC015", CategoryName = "Office Electronics" }
                };

                Category.AddRange(categories);
                SaveChanges();
            }

            if (!Supplier.Any())
            {
                var suppliers = new List<Supplier>
                {
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Apple Inc." },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Samsung Electronics" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Sony Corporation" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Dell Technologies" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Lenovo Group" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "HP Inc." },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Microsoft Corporation" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "LG Electronics" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "ASUS" },
                    new() { SupplierId = Guid.NewGuid().ToString(), SupplierName = "Acer Inc." }
                };

                Supplier.AddRange(suppliers);
                SaveChanges();
            }

            if (!Discount.Any())
            {
                var discounts = new List<Discount>
                {
                    new()
                    {
                        DiscountId = Guid.NewGuid().ToString(),
                        DiscountCode = "SUMMER2024",
                        DiscountAmount = 15,
                        DiscountDateFrom = new DateTime(2024, 6, 1),
                        DiscountDateTo = new DateTime(2024, 9, 1)
                    },
                    new()
                    {
                        DiscountId = Guid.NewGuid().ToString(),
                        DiscountCode = "WINTER2024",
                        DiscountAmount = 20,
                        DiscountDateFrom = new DateTime(2024, 12, 1),
                        DiscountDateTo = new DateTime(2025, 2, 28)
                    },
                    new()
                    {
                        DiscountId = Guid.NewGuid().ToString(),
                        DiscountCode = "NEWYEAR2025",
                        DiscountAmount = 25,
                        DiscountDateFrom = new DateTime(2025, 1, 1),
                        DiscountDateTo = new DateTime(2025, 1, 31)
                    }
                };

                Discount.AddRange(discounts);
                SaveChanges();

                Discount.AddRange(discounts);
                SaveChanges();
            }

            if (!Product.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        NamePr = "Samsung Galaxy S23",
                        NameSerial = "S23-SM-G991B",
                        Detail = "Samsung Galaxy S23 - 128GB, 8GB RAM, Snapdragon 8 Gen 2",
                        Price = 899,
                        QuantityPr = 50,
                        GuaranteePeriod = 24,
                        SupplierId = "1148a964-9c13-4992-a494-ddd5feed48bb",
                        CategoryId = "MOBIL002"
                    },
                    new Product
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        NamePr = "Apple MacBook Air M2",
                        NameSerial = "MBA-M2-2023",
                        Detail = "Apple MacBook Air M2, 13-inch, 512GB SSD, 16GB RAM",
                        Price = 1199,
                        QuantityPr = 30,
                        GuaranteePeriod = 24,
                        SupplierId = "f46e48bf-1eb4-46d7-ae69-b6500103b438",
                        CategoryId = "COMP001"
                    },
                    new Product
                    {
                        ProductId = Guid.NewGuid().ToString(),
                        NamePr = "Sony WH-1000XM5",
                        NameSerial = "WHXM5-2024",
                        Detail = "Sony WH-1000XM5 Wireless Noise Cancelling Headphones",
                        Price = 350,
                        QuantityPr = 100,
                        GuaranteePeriod = 12,
                        SupplierId = "551761f7-f32b-4cff-a91b-9f503a34d92a",
                        CategoryId = "AUDIO003"
                    }
                };

                Product.AddRange(products);
                SaveChanges();
            }
        }
    }
}
