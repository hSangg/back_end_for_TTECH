using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace tech_project_back_end.Extensions
{
    public static class AuthenticationSetup
    {
        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
        {

            services.AddAuthentication().AddJwtBearer(
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

            return services;
        }
    }
}
