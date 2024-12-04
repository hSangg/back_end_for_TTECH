using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<RoleRepository> _logger;

        public RoleRepository(AppDbContext appDbContext, ILogger<RoleRepository> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task<Role> getUserDefaultRole(string userRole) { 
            return await _appDbContext.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.RoleName == userRole);
        }
    }
}
