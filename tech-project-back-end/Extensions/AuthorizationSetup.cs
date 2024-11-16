namespace tech_project_back_end.Extensions
{
    public static class AuthorizationSetup
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ADMIN", policy => policy.RequireRole("ADMIN"));
                options.AddPolicy("USER", policy => policy.RequireRole("USER"));
            });
            return services;
        }
    }
}
