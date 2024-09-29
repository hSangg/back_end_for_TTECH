using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
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
    }
}
