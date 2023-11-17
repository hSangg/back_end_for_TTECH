using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using tech_project_back_end.Data;
using tech_project_back_end.Helpter;
using tech_project_back_end.Models;
using tech_project_back_end.Services;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _appDBContext;
        private readonly IConfiguration _iConfiguration;
        private readonly IEmailService _iEmailService;

        public UserController(AppDbContext appDBContext, IConfiguration configuration, IEmailService emailService)
        {
            this._appDBContext = appDBContext;
            this._iConfiguration = configuration;
            this._iEmailService = emailService;
        }



        [HttpPost("register")]
        public IActionResult Register(User user)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra email đã tồn tại hay chưa
            if (_appDBContext.User.Any(u => u.email == user.email))
            {
                ModelState.AddModelError("email", "Email already exists.");
                return BadRequest(ModelState);
            }

            // Kiểm tra số điện thoại đã tồn tại hay chưa
            if (_appDBContext.User.Any(u => u.phone == user.phone))
            {
                ModelState.AddModelError("phone", "Phone number already exists.");
                return BadRequest(ModelState);
            }

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

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string email)
        {
            try
            {
                var existingUser = await _appDBContext.User.FirstOrDefaultAsync(u => u.email == email);

                if (existingUser == null)
                {
                    return NotFound("User not found");
                }

                // Update the existing User
                string newPassword = Guid.NewGuid().ToString()[..5];
                existingUser.password = BCrypt.Net.BCrypt.HashPassword(newPassword);

                await _appDBContext.SaveChangesAsync();

                MailRequest mailrequest = new MailRequest();
                mailrequest.ToEmail = email;
                mailrequest.Subject = "Đổi mật khẩu";
                mailrequest.Body = "Mật khẩu mới được đổi là: "+ newPassword;
                await _iEmailService.SendEmailAsync(mailrequest);
                return Ok("Password changed");


            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpGet("GetAllUser")]
        public IActionResult GetAllUser()
        {
            var userList = _appDBContext.User.ToList();
            return Ok(userList);

        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


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

        [HttpPut("{id}")]
        public IActionResult Update([FromBody] User updatedUser)
        {
            var user = _appDBContext.User.FirstOrDefault(c => c.user_id == updatedUser.user_id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // Update the values in the existing user record with the new values
            user.name = updatedUser.name;
            user.email = updatedUser.email;
            user.phone = updatedUser.phone;

            // Save changes to the database
            _appDBContext.SaveChanges();

            // Return the updated user and a message indicating success
            return Ok(new { user, message = "User updated successfully" });
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
