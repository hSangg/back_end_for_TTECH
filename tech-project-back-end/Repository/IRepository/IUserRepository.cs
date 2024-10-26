using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository
{
    public interface IUserRepository
    {
        public Task<int> Count();

        public Task<IEnumerable<User>> GetAll();

        public Task<User> GetById(string userid);

        public Task<User> GetByPhone(string phone);

        public Task<User> GetByEmail(string email);

        public Task UpdateUser(User user);

        public Task UpdatePassword(User user, string newPassword);

        public Task AddUser(User user);

        public Task<bool> CheckEmail(string email);

        public Task<bool> CheckPhone(string phone);

        public Task<User> GetUserRolePermissionByPhoneNumber(string phone);
    }
}
