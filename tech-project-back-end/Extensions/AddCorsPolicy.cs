namespace tech_project_back_end.Extensions
{
    public static class AddCorsPolicy
    {
        public static IServiceCollection CorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });

            return services;
        }
    }
}
