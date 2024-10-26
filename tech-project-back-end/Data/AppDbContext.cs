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
            modelBuilder.Entity<Role>().HasKey(c => c.RoleId);
            modelBuilder.Entity<Permission>().HasKey(c => c.PermissionId);
            modelBuilder.Entity<UserRole>()
                        .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

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

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.RoleId, ur.UserId });

                entity.HasOne(userRole => userRole.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(userRole => userRole.UserId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });

                entity.HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>().HasData(
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
                );

            modelBuilder.Entity<Supplier>().HasData(
                    new() { SupplierId = "S01", SupplierName = "Apple Inc." },
                    new() { SupplierId = "S02", SupplierName = "Samsung Electronics" },
                    new() { SupplierId = "S03", SupplierName = "Sony Corporation" },
                    new() { SupplierId = "S04", SupplierName = "Dell Technologies" },
                    new() { SupplierId = "S05", SupplierName = "Lenovo Group" },
                    new() { SupplierId = "S06", SupplierName = "HP Inc." },
                    new() { SupplierId = "S07", SupplierName = "Microsoft Corporation" },
                    new() { SupplierId = "S08", SupplierName = "LG Electronics" },
                    new() { SupplierId = "S09", SupplierName = "ASUS" },
                    new() { SupplierId = "S10", SupplierName = "Acer Inc." }
                );

            modelBuilder.Entity<Discount>().HasData(
                    new()
                    {
                        DiscountId = "DC01",
                        DiscountCode = "SUMMER2024",
                        DiscountAmount = 15,
                        DiscountDateFrom = new DateTime(2024, 6, 1),
                        DiscountDateTo = new DateTime(2024, 9, 1)
                    },
                    new()
                    {
                        DiscountId = "DC02",
                        DiscountCode = "WINTER2024",
                        DiscountAmount = 20,
                        DiscountDateFrom = new DateTime(2024, 12, 1),
                        DiscountDateTo = new DateTime(2025, 2, 28)
                    },
                    new()
                    {
                        DiscountId = "DC03",
                        DiscountCode = "NEWYEAR2025",
                        DiscountAmount = 25,
                        DiscountDateFrom = new DateTime(2025, 1, 1),
                        DiscountDateTo = new DateTime(2025, 1, 31)
                    }
                );

            modelBuilder.Entity<Product>().HasData(
                    new()
                    {
                        ProductId = "1c710c3b-8645-4733-9161-c194debaefac",
                        NamePr = "Samsung Galaxy S23",
                        NameSerial = "S23-SM-G991B",
                        Detail = "Samsung Galaxy S23 - 128GB, 8GB RAM, Snapdragon 8 Gen 2",
                        Price = 899,
                        QuantityPr = 50,
                        GuaranteePeriod = 24,
                        SupplierId = "S02",
                        CategoryId = "MOBIL002"
                    },
                    new()
                    {
                        ProductId = "cfb9782a-c171-4936-a127-f8e51169aa4a",
                        NamePr = "Apple MacBook Air M2",
                        NameSerial = "MBA-M2-2023",
                        Detail = "Apple MacBook Air M2, 13-inch, 512GB SSD, 16GB RAM",
                        Price = 1199,
                        QuantityPr = 30,
                        GuaranteePeriod = 24,
                        SupplierId = "S01",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "e0eb04fa-58f1-474b-b5fe-5dbf5d9b3a21",
                        NamePr = "Sony WH-1000XM5",
                        NameSerial = "WHXM5-2024",
                        Detail = "Sony WH-1000XM5 Wireless Noise Cancelling Headphones",
                        Price = 350,
                        QuantityPr = 100,
                        GuaranteePeriod = 12,
                        SupplierId = "S03",
                        CategoryId = "AUDIO003"
                    },
                    new()
                    {
                        ProductId = "3c8ff97a-834f-46b5-9aa5-e33ab594f28d",
                        NamePr = "iPhone 14",
                        NameSerial = "IP14-A2643",
                        Detail = "Apple iPhone 14 - 256GB, 6GB RAM, A15 Bionic",
                        Price = 1099,
                        QuantityPr = 30,
                        GuaranteePeriod = 24,
                        SupplierId = "S01",
                        CategoryId = "MOBIL002"
                    },
                    new()
                    {
                        ProductId = "c58b686b-d1e7-4996-b99b-4a9c7d47cb8c",
                        NamePr = "MacBook Pro 14",
                        NameSerial = "MBP14-M2-2023",
                        Detail = "Apple MacBook Pro 14 - 1TB SSD, 16GB RAM, M2",
                        Price = 2499,
                        QuantityPr = 15,
                        GuaranteePeriod = 36,
                        SupplierId = "S01",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "6a1325b8-0241-4d41-8bc8-ccf8f8d5e4a6",
                        NamePr = "Samsung QLED TV",
                        NameSerial = "QLED-TV-Q80A",
                        Detail = "Samsung QLED 4K TV - 65 inch, HDR, Quantum Processor 4K",
                        Price = 1999,
                        QuantityPr = 20,
                        GuaranteePeriod = 12,
                        SupplierId = "S02",
                        CategoryId = "TV008"
                    },
                    new()
                    {
                        ProductId = "24df5fdd-3153-4541-b80e-e1832f81e9ff",
                        NamePr = "Sony PlayStation 5",
                        NameSerial = "PS5-CFI-1016A",
                        Detail = "Sony PlayStation 5 - Ultra HD Blu-ray, 825GB SSD",
                        Price = 499,
                        QuantityPr = 100,
                        GuaranteePeriod = 12,
                        SupplierId = "S03",
                        CategoryId = "GAME004"
                    },
                    new()
                    {
                        ProductId = "4c9266f7-b03b-44ec-8775-5e6a7ae154cd",
                        NamePr = "Dell XPS 13",
                        NameSerial = "XPS13-9310",
                        Detail = "Dell XPS 13 - 512GB SSD, 16GB RAM, Intel i7-1165G7",
                        Price = 1399,
                        QuantityPr = 20,
                        GuaranteePeriod = 24,
                        SupplierId = "S04",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "f2b7d5b2-6506-409b-a2b8-d889b97299fe",
                        NamePr = "Dell UltraSharp Monitor",
                        NameSerial = "U2719D",
                        Detail = "Dell UltraSharp 27-inch Monitor - QHD, USB-C",
                        Price = 499,
                        QuantityPr = 30,
                        GuaranteePeriod = 12,
                        SupplierId = "S04",
                        CategoryId = "ACCES013"
                    },
                    new()
                    {
                        ProductId = "cde29234-2be7-4e84-b3b6-5d6b48b95a9d",
                        NamePr = "Lenovo ThinkPad X1 Carbon",
                        NameSerial = "TPX1C-9G",
                        Detail = "Lenovo ThinkPad X1 Carbon - 1TB SSD, 16GB RAM, Intel i7",
                        Price = 1799,
                        QuantityPr = 10,
                        GuaranteePeriod = 24,
                        SupplierId = "S05",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "f61dbaf5-fad8-4e18-a882-3ecbc674fc1e",
                        NamePr = "Lenovo Legion 5",
                        NameSerial = "LEGION5-2022",
                        Detail = "Lenovo Legion 5 - 512GB SSD, 16GB RAM, AMD Ryzen 7",
                        Price = 1299,
                        QuantityPr = 15,
                        GuaranteePeriod = 24,
                        SupplierId = "S05",
                        CategoryId = "GAME004"
                    },
                    new()
                    {
                        ProductId = "60de68ab-e7e2-48c2-bec9-8b6e8f55c746",
                        NamePr = "HP Spectre x360",
                        NameSerial = "SPECTRE-X360",
                        Detail = "HP Spectre x360 - 1TB SSD, 16GB RAM, Intel i7",
                        Price = 1599,
                        QuantityPr = 12,
                        GuaranteePeriod = 24,
                        SupplierId = "S06",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "c982b1b9-d0df-49d4-b5d9-ef99d708b774",
                        NamePr = "HP Envy 13",
                        NameSerial = "ENVY13-2023",
                        Detail = "HP Envy 13 - 512GB SSD, 8GB RAM, Intel i5",
                        Price = 999,
                        QuantityPr = 25,
                        GuaranteePeriod = 24,
                        SupplierId = "S06",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "5b98a7a8-34a0-4fd2-972b-29c79b2db755",
                        NamePr = "Microsoft Surface Laptop 4",
                        NameSerial = "SURF-LAP4",
                        Detail = "Microsoft Surface Laptop 4 - 512GB SSD, 16GB RAM, Intel i7",
                        Price = 1399,
                        QuantityPr = 20,
                        GuaranteePeriod = 24,
                        SupplierId = "S07",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "fa7b321c-8cbf-4a25-9a97-d526cd02bc9f",
                        NamePr = "Microsoft Surface Pro 8",
                        NameSerial = "SURF-PRO8",
                        Detail = "Microsoft Surface Pro 8 - 256GB SSD, 8GB RAM, Intel i5",
                        Price = 1099,
                        QuantityPr = 25,
                        GuaranteePeriod = 24,
                        SupplierId = "S07",
                        CategoryId = "MOBIL002"
                    },
                    new()
                    {
                        ProductId = "d74f7c5b-b64b-40f9-b582-2b31d0d1ed94",
                        NamePr = "LG OLED TV CX",
                        NameSerial = "OLED55CXPUA",
                        Detail = "LG OLED TV CX - 55 inch, 4K, Dolby Vision, AI ThinQ",
                        Price = 1899,
                        QuantityPr = 15,
                        GuaranteePeriod = 24,
                        SupplierId = "S08",
                        CategoryId = "TV008"
                    },
                    new()
                    {
                        ProductId = "497cc96a-65e7-4742-a405-d61f8ffda9c6",
                        NamePr = "LG Gram 17",
                        NameSerial = "GRAM17-2022",
                        Detail = "LG Gram 17 - 1TB SSD, 16GB RAM, Intel i7",
                        Price = 1599,
                        QuantityPr = 10,
                        GuaranteePeriod = 24,
                        SupplierId = "S08",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "93bba2e2-1b2e-4c4f-9fbf-58ff0a3a23cc",
                        NamePr = "ASUS ROG Zephyrus G14",
                        NameSerial = "ROG-G14-2022",
                        Detail = "ASUS ROG Zephyrus G14 - 1TB SSD, 16GB RAM, AMD Ryzen 9",
                        Price = 1799,
                        QuantityPr = 12,
                        GuaranteePeriod = 24,
                        SupplierId = "S09",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "3f747674-b515-4f30-b2ad-4b50520d6ae5",
                        NamePr = "ASUS VivoBook 15",
                        NameSerial = "VB15-2023",
                        Detail = "ASUS VivoBook 15 - 512GB SSD, 8GB RAM, Intel i5",
                        Price = 899,
                        QuantityPr = 30,
                        GuaranteePeriod = 24,
                        SupplierId = "S09",
                        CategoryId = "COMP001"
                    },
                    new()
                    {
                        ProductId = "762d1d77-10c9-4c1b-b7a6-ef6f4a87082f",
                        NamePr = "Acer Predator Helios 300",
                        NameSerial = "PH315-53",
                        Detail = "Acer Predator Helios 300 - 1TB SSD, 16GB RAM, Intel i7",
                        Price = 1499,
                        QuantityPr = 10,
                        GuaranteePeriod = 24,
                        SupplierId = "S10",
                        CategoryId = "GAME004"
                    },
                    new()
                    {
                        ProductId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b",
                        NamePr = "Acer Aspire 5",
                        NameSerial = "ASPIRE5-2023",
                        Detail = "Acer Aspire 5 - 512GB SSD, 8GB RAM, Intel i5",
                        Price = 749,
                        QuantityPr = 20,
                        GuaranteePeriod = 24,
                        SupplierId = "S10",
                        CategoryId = "COMP001"
                    }
                );

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b",
                    RoleName = "ADMIN"

                },
                new Role
                {
                    RoleId = "3f747674-b515-4f30-b2ad-4b50520d6ae5",
                    RoleName = "USER"

                }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission
                {
                    PermissionId = "3f747674-b515-4f30-b2ad-4b50520d6ae5",
                    PermissionName = "view_product"
                },
                new Permission
                {
                    PermissionId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b",
                    PermissionName = "update_product"
                }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    PermissionId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b",
                    RoleId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b"

                },
                new RolePermission
                {
                    PermissionId = "3f747674-b515-4f30-b2ad-4b50520d6ae5",
                    RoleId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b"

                },
                new RolePermission
                {
                    PermissionId = "3f747674-b515-4f30-b2ad-4b50520d6ae5",
                    RoleId = "3f747674-b515-4f30-b2ad-4b50520d6ae5"
                }
            );

            modelBuilder.Entity<Image>().HasData(
                    new Image
                    {
                        ImageId = 1,
                        ProductId = "cfb9782a-c171-4936-a127-f8e51169aa4a",
                        ImageHref = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mba13-midnight-select-202402?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1708367688034"
                    },
                    new Image
                    {
                        ImageId = 2,
                        ProductId = "e0eb04fa-58f1-474b-b5fe-5dbf5d9b3a21",
                        ImageHref = "https://www.sony.com.vn/image/6145c1d32e6ac8e63a46c912dc33c5bb?fmt=pjpeg&wid=330&bgcolor=FFFFFF&bgc=FFFFFF"
                    },
                    new Image
                    {
                        ImageId = 3,
                        ProductId = "1c710c3b-8645-4733-9161-c194debaefac",
                        ImageHref = "https://cdn2.cellphones.com.vn/insecure/rs:fill:0:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/s/a/samsung-s23_1.png"
                    },
                    new Image
                    {
                        ImageId = 4,
                        ProductId = "3c8ff97a-834f-46b5-9aa5-e33ab594f28d",
                        ImageHref = "https://cdn.tgdd.vn/Products/Images/42/240259/iPhone-14-thumb-tim-1-600x600.jpg"
                    },
                    new Image
                    {
                        ImageId = 5,
                        ProductId = "c58b686b-d1e7-4996-b99b-4a9c7d47cb8c",
                        ImageHref = "https://store.storeimages.cdn-apple.com/8756/as-images.apple.com/is/mbp14-spacegray-select-202310?wid=904&hei=840&fmt=jpeg&qlt=90&.v=1697230830200"
                    },
                    new Image
                    {
                        ImageId = 7,
                        ProductId = "6a1325b8-0241-4d41-8bc8-ccf8f8d5e4a6",
                        ImageHref = "https://images.samsung.com/is/image/samsung/p6pim/vn/qa98q80cakxxv/gallery/vn-qled-q80c-451227-qa98q80cakxxv-536734797?$650_519_PNG$"
                    },
                    new Image
                    {
                        ImageId = 8,
                        ProductId = "24df5fdd-3153-4541-b80e-e1832f81e9ff",
                        ImageHref = "https://www.droidshop.vn/wp-content/uploads/2024/09/May-choi-game-Sony-PS5-Pro.jpg"
                    },
                    new Image
                    {
                        ImageId = 10,
                        ProductId = "4c9266f7-b03b-44ec-8775-5e6a7ae154cd",
                        ImageHref = "https://product.hstatic.net/1000331874/product/dell_xps_9320_722c6227ecc04c648c670f1bbb7b7cfd_1024x1024.jpg"
                    },
                    new Image
                    {
                        ImageId = 11,
                        ProductId = "f2b7d5b2-6506-409b-a2b8-d889b97299fe",
                        ImageHref = "https://bizweb.dktcdn.net/thumb/1024x1024/100/329/122/products/man-hinh-4k-dell-ultrasharp-27-inch-ips-60hz-u2723qe-2.jpg?v=1712419236610"
                    },
                    new Image
                    {
                        ImageId = 12,
                        ProductId = "cde29234-2be7-4e84-b3b6-5d6b48b95a9d",
                        ImageHref = "https://minhvu.vn/thumb/lenovothinkpadx1carbonyoga/thinkpadx1carbongen11/lenovothinkpadx1carbongen11cbfjhc_480_360.jpg"
                    },
                    new Image
                    {
                        ImageId = 13,
                        ProductId = "f61dbaf5-fad8-4e18-a882-3ecbc674fc1e",
                        ImageHref = "https://ttcenter.com.vn/uploads/product/2qygkhyp-1249-lenovo-legion-5-y7000p-16irx9-2024-core-i7-14650hx-ram-16gb-ssd-1tb-rtx-4050-16-2-5k-165hz-new.png"
                    },
                    new Image
                    {
                        ImageId = 14,
                        ProductId = "60de68ab-e7e2-48c2-bec9-8b6e8f55c746",
                        ImageHref = "https://gamalaptop.vn/wp-content/uploads/2021/07/HP-Spectre-x360-15-01.jpg"
                    },
                    new Image
                    {
                        ImageId = 15,
                        ProductId = "c982b1b9-d0df-49d4-b5d9-ef99d708b774",
                        ImageHref = "https://cdn.tgdd.vn/Products/Images/44/217269/hp-envy-13-aq1022tu-i5-10210u-8gb-512gb-win10-8qn-9-217269-600x600.jpg"
                    },
                    new Image
                    {
                        ImageId = 16,
                        ProductId = "5b98a7a8-34a0-4fd2-972b-29c79b2db755",
                        ImageHref = "https://ttcenter.com.vn/uploads/product/bskqzvlw-1058-surface-laptop-4-ryzen-5-4680u-8gb-128gb-amd-radeon-graphics-13-5-2k.jpg"
                    },
                    new Image
                    {
                        ImageId = 17,
                        ProductId = "fa7b321c-8cbf-4a25-9a97-d526cd02bc9f",
                        ImageHref = "https://surfacezone.vn/wp-content/uploads/2022/02/RE4P7Wb.png"
                    },
                    new Image
                    {
                        ImageId = 18,
                        ProductId = "d74f7c5b-b64b-40f9-b582-2b31d0d1ed94",
                        ImageHref = "https://media.us.lg.com/transform/ecomm-PDPGallery-1100x730/42e5394e-7250-4d5e-9ed2-03d618545268/md07501037-large01-jpg"
                    },
                    new Image
                    {
                        ImageId = 19,
                        ProductId = "497cc96a-65e7-4742-a405-d61f8ffda9c6",
                        ImageHref = "https://bcavn.com/Image/Picture/New/17Z90R.jpg"
                    },
                    new Image
                    {
                        ImageId = 20,
                        ProductId = "93bba2e2-1b2e-4c4f-9fbf-58ff0a3a23cc",
                        ImageHref = "https://dlcdnwebimgs.asus.com/gain/3D241166-0518-4745-B481-D901886BFD14"
                    },
                    new Image
                    {
                        ImageId = 21,
                        ProductId = "3f747674-b515-4f30-b2ad-4b50520d6ae5",
                        ImageHref = "https://dlcdnwebimgs.asus.com/gain/5707c7d1-7e0f-4897-bc93-d31d017b2edf/w800"
                    },
                    new Image
                    {
                        ImageId = 22,
                        ProductId = "762d1d77-10c9-4c1b-b7a6-ef6f4a87082f",
                        ImageHref = "https://www.acervietnam.com.vn/wp-content/uploads/2021/08/predator-helios-300-ph315-55-4zone-backlit-on-black-05-min.png"
                    },
                    new Image
                    {
                        ImageId = 23,
                        ProductId = "9c49a4fa-0eac-4e44-a1e0-28e3e2bca66b",
                        ImageHref = "https://cdn.tgdd.vn/Products/Images/44/314630/acer-aspire-5-a515-58gm-51lb-i5-nxkq4sv002-170923-015941-600x600.jpg"
                    }

                );
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
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }


    }
}
