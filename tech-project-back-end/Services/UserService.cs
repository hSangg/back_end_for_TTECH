using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using dotenv.net;
using Irony.Parsing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using tech_project_back_end.DTO;
using tech_project_back_end.DTO.Users;
using tech_project_back_end.Helpter;
using tech_project_back_end.Models;
using tech_project_back_end.Repository;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(IMapper mapper, IUserRepository userRepository, IEmailService emailService, ILogger<UserService> logger, IConfiguration configuration)
        {
            _userRepository = userRepository; ;
            _emailService = emailService;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<int> GetTotalUser()
        {
            return await _userRepository.Count();
        }

        public async Task<UserDTO> GetUserById(string userid)
        {
            try
            {
                var user = await _userRepository.GetById(userid);
                return user != null ? _mapper.Map<UserDTO?>(user) : null;
            }
            catch
            {
                _logger.LogError("Error while fetching user with id: ", userid);
                throw;
            }
        }

        public async Task<List<UserDTO>> GetAllUser()
        {
            try
            {
                var userList = await _userRepository.GetAll();
                return _mapper.Map<List<UserDTO>>(userList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching user list");
                throw;
            }
        }

        public async Task<(bool Success, string? Message, UserDTO? User, string? Token)> Login(UserLoginDTO userLogin)
        {
            var user = await _userRepository.GetByPhone(userLogin.Phone);

            if (user == null)
            {
                return (false, "User not found", null, null);
            }

            if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                return (false, "Wrong password", null, null);
            }

            string token = CreateToken(user);

            return (true, null, _mapper.Map<UserDTO>(user), token);
        }

        public async Task<(bool Success, string? Message, UserDTO? User)> UpdateUser(UserUpdateDTO updatedUser)
        {
            var user = await _userRepository.GetById(updatedUser.UserId);

            if (user == null)
            {
                return (false, "User not found", null);
            }

            user.Name = updatedUser.Name;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;

            await _userRepository.UpdateUser(user);

            return (true, "User updated successfully", _mapper.Map<UserDTO>(user));
        }

        public async Task<(bool Success, string? Message)> ForgetPassword(string email)
        {
            var existingUser = await _userRepository.GetByEmail(email);
            if (existingUser == null)
            {
                return (false, "User not found");
            }

            string newPassword = Guid.NewGuid().ToString()[..5];
            
            await _userRepository.UpdatePassword(existingUser, newPassword);

            var emailBody = _emailService.ProcessEmail("EmailResetPassword", new Dictionary<string, string>
            {
                { "Phone", existingUser.Phone },
                { "NewPassword", newPassword }
            });

            MailRequest mailRequest = new MailRequest
            {
                ToEmail = email,
                Subject = "Đổi mật khẩu",
                Body = emailBody
            };

            await _emailService.SendEmailAsync(mailRequest);

            return (true, "Password changed successfully");
        }

        public async Task<(bool Success, string? Message, UserDTO? User, string? Token)> Register(UserDTO userRegister)
        {
            if (await _userRepository.CheckEmail(userRegister.Email))
            {
                return (false, "Email already exists", null, null);
            }

            if (await _userRepository.CheckPhone(userRegister.Phone))
            {
                return (false, "Phone number already exists", null, null);
            }

            var user = new User
            {
                Name = userRegister.Name,
                Email = userRegister.Email,
                Phone = userRegister.Phone,
                CreatedAt = DateTime.Now,
                Password = BCrypt.Net.BCrypt.HashPassword(userRegister.Password)
            };

            await _userRepository.AddUser(user);

            string token = CreateToken(user);

            return (true, "User registered successfully", _mapper.Map<UserDTO>(user), token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
               Environment.GetEnvironmentVariable("ASPNETCORE_AUTHENTICATION_SCHEMES_BEARER_SIGNINGKEYS")));

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
