using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models.User;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<SupplierRepository> _logger;

        public UserRepository(AppDbContext appDbContext, ILogger<SupplierRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<int> Count()
        {
            return await _appDbContext.User.CountAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _appDbContext.User.ToListAsync();
        }

        public async Task<User> GetById(string userid)
        {
            return await _appDbContext.User
                .FirstOrDefaultAsync(user => user.UserId == userid);
        }

        public async Task<User> GetByPhone(string phone)
        {
            return await _appDbContext.User.
                FirstOrDefaultAsync(user => user.Phone == phone);
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _appDbContext.User.
                FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task UpdateUser(User user)
        {
            _appDbContext.User.Update(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdatePassword(User user, string newPassword)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task AddUser(User user)
        {
            await _appDbContext.User.AddAsync(user);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckEmail(string email)
        {
            return await _appDbContext.User.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> CheckPhone(string phone)
        {
            return await _appDbContext.User.AnyAsync(u => u.Phone == phone);
        }
    }
}
