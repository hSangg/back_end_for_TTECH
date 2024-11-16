using tech_project_back_end.Helpter;
using tech_project_back_end.Repositories;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Repository;
using tech_project_back_end.Services.IService;
using tech_project_back_end.Services;
using Microsoft.Extensions.Configuration;

namespace tech_project_back_end.Extensions
{
    public static class ServiceSetup
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.Configure<EMailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped<IProductService, ProductService>();

            services.AddTransient<IProductRepository, ProductRepository>();

            services.AddSingleton<ILogger>(provider =>
               provider.GetRequiredService<ILogger<ProductService>>());

            services.AddScoped<IRevenueService, RevenueService>();

            services.AddSingleton<ILogger>(provider =>
               provider.GetRequiredService<ILogger<RevenueService>>());

            services.AddScoped<IUserService, UserService>();

            services.AddTransient<IUserRepository, UserRepository>();

            services.AddSingleton<ILogger>(provider =>
               provider.GetRequiredService<ILogger<UserService>>());

            services.AddScoped<ISupplierService, SupplierService>();

            services.AddTransient<ISupplierRepository, SupplierRepository>();
            
            services.AddSingleton<ILogger>(provider => 
                provider.GetRequiredService<ILogger<SupplierService>>());

            services.AddScoped<IOrderService, OrderService>();

            services.AddTransient<IOrderRepository, OrderRepository>();

            services.AddSingleton<ILogger>(provider => 
                provider.GetRequiredService<ILogger<OrderService>>());
            
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IDiscountRepository, DiscountRepository>();

            services.AddScoped<IDiscountService, DiscountService>();

            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<ICartService, CartService>();

            services.AddScoped<IDetailOrderRepository, DetailOrderRepository>();

            services.AddScoped<IDetailOrderService, DetailOrderService>();

            services.AddTransient<IEmailService, EmailService>();

            services.AddScoped<ICategoryService, CategoryService>();

            services.AddTransient<ICategoryRepository, CategoryRepository>();

            services.AddTransient<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}
