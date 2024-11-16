using CloudinaryDotNet;
using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using tech_project_back_end.Data;
using tech_project_back_end.Extensions;

var builder = WebApplication.CreateBuilder(args);

DotEnv.Load();

builder.Services.CorsPolicy();

var handler = new HttpClientHandler()
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
};

builder.Services.AddSingleton(new HttpClient(handler));

builder.Services.AddDatabaseAndCloud(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers();

builder.Services.AddMvc();

builder.Services.AddSwaggerDocumentation();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddCustomAuthentication();

builder.Services.AddCustomAuthorization();

builder.Services.AddCustomServices(builder.Configuration);

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

