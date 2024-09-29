namespace tech_project_back_end.Repository.IRepository
{
    public interface IUserRepository
    {
        public Task<int> Count();
    }
}
