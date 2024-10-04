using tech_project_back_end.DTO;
using tech_project_back_end.DTO.Users;
using tech_project_back_end.Models;

namespace tech_project_back_end.Services.IService
{
    public interface IUserService
    {
        public Task<int> GetTotalUser();
        public Task<List<UserDTO>> GetAllUser();
        public Task<UserDTO> GetUserById(string userid);
        public Task<(bool Success, string? Message, UserDTO? User, string? Token)> Login(UserLoginDTO userLogin);
        public Task<(bool Success, string? Message, UserDTO? User)> UpdateUser(UserUpdateDTO updatedUser);
        public Task<(bool Success, string? Message)> ForgetPassword(string email);
        public Task<(bool Success, string? Message, UserDTO? User, string? Token)> Register(UserDTO userRegisterDto);
        public Task<string> CreateToken(User user);
        public string GetEmailTemplate(string templateName);
        public string ReplacePlaceholders(string template, Dictionary<string, string> placeholders);
    }
}
