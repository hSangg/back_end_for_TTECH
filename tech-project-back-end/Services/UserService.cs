using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using dotenv.net;
using Irony.Parsing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using tech_project_back_end.Data;
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
        private readonly IRoleRepository _roleRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public UserService(IMapper mapper, 
            IUserRepository userRepository, 
            IRoleRepository roleRepository,
            IEmailService emailService, 
            ILogger<UserService> logger, 
            IConfiguration configuration,
            IMemoryCache cache)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _emailService = emailService;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _cache = cache;
        }

        private const int LockoutDurationMinutes = 1;
        private const int MaxFailedAttempts = 3;

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
            var user = await GetUserByPhoneNumberWithRolesAndPermissions(userLogin.Phone);

            if (user == null)
            {
                return (false, "User not found", null, null);
            }

            if (_cache.TryGetValue($"Lockout_{userLogin.Phone}", out _))
            {
                return (false, "Account locked. Try again in 1 minute.", null, null);

            }

            if (!BCrypt.Net.BCrypt.Verify(userLogin.Password, user.Password))
            {
                int attempts = _cache.GetOrCreate($"FailedAttempts_{userLogin.Phone}", entry =>
                {
                    entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                    return 0;
                });
                attempts++;

                if (attempts > MaxFailedAttempts)
                {
                    _cache.Set($"Lockout_{userLogin.Phone}", true, TimeSpan.FromMinutes(LockoutDurationMinutes));
                    _cache.Remove($"FailedAttempts_{userLogin.Phone}");
                    return (false, "Account locked. Try again in 1 minute.", null, null);
                } else
                {
                    _cache.Set($"Lockout_{userLogin.Phone}", attempts);
                    return (false, $"Wrong password. {MaxFailedAttempts - attempts} attempts remaining.", null, null);
                }
            }

            _cache.Remove($"FailedAttempts_{userLogin.Phone}");

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

            var defaultRole = await _roleRepository.getUserDefaultRole();


            if (defaultRole == null) {
                throw new Exception("User default role not found");
            }

            user.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    User = user,
                    Role = defaultRole,
                    RoleId = defaultRole.RoleId,
                    UserId = user.UserId
                }
            };

            await _userRepository.AddUser(user);

            var userWithRolesPermissions = await GetUserByPhoneNumberWithRolesAndPermissions(user.Phone);

            string token = CreateToken(userWithRolesPermissions);

            return (true, "User registered successfully", _mapper.Map<UserDTO>(user), token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
            };

            if (user.Roles != null)
            {
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            if (user.Permissions != null)
            {
                foreach (var permission in user.Permissions)
                {
                    claims.Add(new Claim("Permission", permission));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
               Environment.GetEnvironmentVariable("ASPNETCORE_AUTHENTICATION_SCHEMES_BEARER_SIGNINGKEYS")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private async Task<User> GetUserByPhoneNumberWithRolesAndPermissions(string phoneNumber)
        {
            var user = await _userRepository.GetUserRolePermissionByPhoneNumber(phoneNumber);

            if (user != null)
            {
                user.Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
                user.Permissions = user.UserRoles
                                       .SelectMany(ur => ur.Role.RolePermissions)
                                       .Select(rp => rp.Permission.PermissionName)
                                       .Distinct()
                                       .ToList();
            }

            return user;
        }
    }
}
