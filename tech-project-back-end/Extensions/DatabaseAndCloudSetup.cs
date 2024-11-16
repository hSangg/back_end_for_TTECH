using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;

namespace tech_project_back_end.Extensions
{
    public static class DatabaseAndCloudSetup
    {
        public static IServiceCollection AddDatabaseAndCloud(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("AppDbConnectionString");
            services.AddDbContext<AppDbContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            Cloudinary cloudinary = new Cloudinary(
                Environment.GetEnvironmentVariable("CLOUDINARY_URL")
            );
            cloudinary.Api.Secure = true;
            services.AddSingleton(cloudinary);

            return services;
        }
    }
}
