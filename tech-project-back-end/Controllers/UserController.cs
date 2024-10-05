using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using tech_project_back_end.Data;
using tech_project_back_end.DTO;
using tech_project_back_end.DTO.Users;
using tech_project_back_end.Helpter;
using tech_project_back_end.Models;
using tech_project_back_end.Services;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public UserController(IUserService userService, ILogger logger)
        {
            this._userService = userService;
            this._logger = logger;
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
                if (user != null)
                    return Ok(user);
                else return NotFound($"There's no user with {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There's been error while fetching user with id: {userId}");
                throw;
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                (bool success, string? message, UserDTO? userRegistered, string? token) = await _userService.Register(user);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, message);
                    return BadRequest(ModelState);
                }

                Response.Cookies.Append("token", token!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddHours(1)
                });

                return CreatedAtAction("AddUser", new { id = userRegistered!.UserId }, new { userRegistered, token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user");
                throw;
            } 
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] string email)
        {
            try
            {
                (bool success, string? message) = await _userService.ForgetPassword(email);

                if (!success)
                {
                    return NotFound(message);
                }

                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while sending email");
                throw;
            }
        }

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var userList = await _userService.GetAllUser();
                return Ok(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user list");
                throw;
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                (bool success, string? message, UserDTO loginedUser, string token) = await _userService.Login(user);

                if (!success)
                {
                    return NotFound(message);
                }

                Response.Cookies.Append("token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.Now.AddHours(1)
                });
                return Ok(new { loginedUser, token });
            }
            catch(Exception ex) {
                _logger.LogError(ex, "Error occurred while login");
                throw;
            }
        }

        [HttpPut("UpdateUserInfor")]
        public async Task<IActionResult> UpdateUserInfor([FromBody] UserUpdateDTO updatedUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            (bool success, string? message, UserDTO? user) = await _userService.UpdateUser(updatedUser);

            if (!success)
            {
                return NotFound(message);
            }

            return Ok(new { user, message });
        }
    }
}
