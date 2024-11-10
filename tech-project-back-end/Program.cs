using CloudinaryDotNet;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using tech_project_back_end.Data;
using tech_project_back_end.Helpter;
using tech_project_back_end.Repositories;
using tech_project_back_end.Repository;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services;
using tech_project_back_end.Services.IService;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var handler = new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
};

builder.Services.AddSingleton(new HttpClient(handler));

var connectionString = builder.Configuration.GetConnectionString("AppDbConnectionString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddMvc();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
    ;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("view_product", policy => policy.RequireClaim("Permission", "view_product"));
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddAuthentication().AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                 Environment.GetEnvironmentVariable("ASPNETCORE_AUTHENTICATION_SCHEMES_BEARER_SIGNINGKEYS")))
        };
    }
);

builder.Services.AddSingleton<ILogger>(provider =>
   provider.GetRequiredService<ILogger<OrderService>>());

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddTransient<IProductRepository, ProductRepository>();

builder.Services.AddSingleton<ILogger>(provider =>
   provider.GetRequiredService<ILogger<ProductService>>());

builder.Services.AddScoped<IRevenueService, RevenueService>();

builder.Services.AddTransient<IOrderRepository, OrderRepository>();

builder.Services.AddSingleton<ILogger>(provider =>
   provider.GetRequiredService<ILogger<RevenueService>>());

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();

builder.Services.AddSingleton<ILogger>(provider =>
   provider.GetRequiredService<ILogger<UserService>>());

builder.Services.AddScoped<ISupplierService, SupplierService>();

builder.Services.AddTransient<ISupplierRepository, SupplierRepository>();

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();

builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddScoped<ICartService, CartService>();

builder.Services.AddScoped<IDetailOrderRepository, DetailOrderRepository>();

builder.Services.AddScoped<IDetailOrderService, DetailOrderService>();

builder.Services.AddSingleton<ILogger>(provider => provider.GetRequiredService<ILogger<SupplierService>>());

builder.Services.Configure<EMailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();

builder.Services.AddTransient<IRoleRepository, RoleRepository>();

Cloudinary cloudinary = new Cloudinary(
    Environment.GetEnvironmentVariable("CLOUDINARY_URL")
    );
cloudinary.Api.Secure = true;
builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

app.UseSwagger();

app.UseSwaggerUI();

app.UseCors("AllowSpecificOrigin");

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

