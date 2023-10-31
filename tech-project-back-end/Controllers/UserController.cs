using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDBContext;
        private readonly IConfiguration _iConfiguration;

        public UserController(AppDbContext appDBContext, IConfiguration configuration)
        {
            this._appDBContext = appDBContext;
            this._iConfiguration = configuration;
        }

      

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);
            _appDBContext.User.Add(user);
            _appDBContext.SaveChanges();

            string token = CreateToken(user);
            string userJson = JsonConvert.SerializeObject(user);


            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            


            return Ok(new { user, token });

        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin user)
        {
            var isExitUser = _appDBContext.User.FirstOrDefault(c => c.phone == user.phone);
            if (isExitUser == null) { return NotFound("User not found"); }

            if (!BCrypt.Net.BCrypt.Verify(user.password,isExitUser.password ))
            {
                return BadRequest("Wrong password.");
            }

            string token = CreateToken(isExitUser);

            string userJson = JsonConvert.SerializeObject(user);

           


            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true, 
                Secure = true,   
                SameSite = SameSiteMode.Strict, 
                Expires = DateTimeOffset.Now.AddHours(1) 
            });


            return Ok(new { user = isExitUser, token });
        }



        private string CreateToken(User user)
        {
            List<Claim>  claims = new List<Claim>(){
            new Claim(ClaimTypes.Name, user.name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                 _iConfiguration.GetSection("Authentication:Schemes:Bearer:SigningKeys:0:Value").Value!));


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
