using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Models.ViewModel;

namespace tech_project_back_end.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<TopSellerProductDTO>> TopSeller(int count);

        Task AddProductAsync(Product product);

        Task<ProductDTO> GetProductByIdAsync(string id);

        Task<FilteredProductResponse> GetFilteredProductsAsync(Filter filter);

        Task<List<ProductBySearchQueryModel>> GetProductBySearchQuery(string keyword);

        Task DeleteProductAsync(string productId);

        Task<List<Image>> GetProductImagesAsync(string productId);

        Task AddImagesAsync(Image image);

        Task UpdateProductAsync(ProductDTO product);

        Task DeleteImageAsync(string productId);
    }
}
