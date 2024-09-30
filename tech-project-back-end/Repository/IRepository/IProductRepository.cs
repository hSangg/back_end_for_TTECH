using tech_project_back_end.DTO;

namespace tech_project_back_end.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<TopSellerProductDTO>> TopSeller(int count);
    }
}
