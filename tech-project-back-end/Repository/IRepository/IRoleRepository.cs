using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository
{
    public interface IRoleRepository
    {
        public Task<Role> getUserDefaultRole(string roleUser); 
    }
}
