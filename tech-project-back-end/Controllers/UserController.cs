using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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

        [HttpGet("GetUserById")]
        public IActionResult GetUserById(string userId)
        {
            var user = _appDBContext.User.Where(user => user.user_id == userId).FirstOrDefault();
            if (user != null) return Ok(user);
            return NotFound("User not found");
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

            user.create_at = DateTime.Now;
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
        public async Task<IActionResult> ForgetPassword([FromBody] string email)
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
                mailrequest.Body = @"
                    <!DOCTYPE html>
                    <html>
                    <head>
	                    <meta charset='UTF-8' />
	                    <meta name='viewport' content='width=device-width, initial-scale=1.0' />
	                    <title>Password Reset Email</title>
	                    <style type='text/css'>
		                    body {
			                    margin: 0;
			                    padding: 0;
			                    font-family: Arial, sans-serif;
			                    line-height: 1.6;
			                    color: #333;
		                    }
		                    h1 {
			                    text-align: center;
			                    margin-top: 50px;
		                    }
		                    p {
			                    text-align: center;
			                    margin-top: 20px;
		                    }
		                    .btn {
			                    background-color: #3b82f6;
			                    color: white !important;
			                    padding: 10px 20px;
			                    border: none;
			                    cursor: pointer;
			                    width: 100%;
			                    margin-top: 30px;
			                    border-radius: 5px;
                                text-decoration: none;
                            
		                    }
		                    .btn:hover {
			                    background-color: #60a5fa;
		                    }
	                    </style>
                    </head>
                    <body>
	                    <h1>Đổi mật khẩu</h1>
	                    <p>Xin chào, <strong>" + existingUser.phone + @"</strong></p>
	                    <p>Chúng tôi đã đổi mật khẩu của tài khoản của bạn do yêu cầu đổi mật khẩu. Mật khẩu mới của bạn là: </p>
	                    <p><strong>" + newPassword + @"</strong></p>
	                    <p>Vui lòng đăng nhập với mật khẩu mới để tiếp tục sử dụng dịch vụ của chúng tôi.</p>
	                    <a href='https://github.com/hSangg' class='btn'>HSang</a>
                    </body>
                    </html>
                    ";

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

            if (!BCrypt.Net.BCrypt.Verify(user.password, isExitUser.password))
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

        [HttpPut("UpdateUserInfor")]
        public IActionResult UpdateUserInfor([FromBody] UpdatedUser updatedUser)
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
            List<Claim> claims = new List<Claim>(){
            new Claim(ClaimTypes.Name, user.name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                 _iConfiguration.GetSection("Authentication:Schemes:Bearer:SigningKeys:0:Value").Value!));


            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(99),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

    }
}
