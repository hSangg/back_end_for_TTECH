using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<int> GetTotalUser()
        {
            return await _userRepository.Count();
        }
    }
}
