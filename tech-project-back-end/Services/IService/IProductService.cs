using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Models.ViewModel;

namespace tech_project_back_end.Services.IService
{
    public interface IProductService
    {
        Task<List<TopSellerProductDTO>> GetTopSellerProducts(int count);

        Task AddProductAsync(CreateProductDTO productDTO);

        Task<ProductDTO> GetProductByIdAsync(string id);

        Task<FilteredProductResponse> GetFilteredProductsAsync(Filter filter);

        Task<List<ProductBySearchQueryModel>> GetProductBySearchQueryAsync(string searchQuery);

        Task DeleteProductAsync(string productId);

        Task<List<ImageDTO>> GetProductImagesAsync(string productId);

        Task AddImagesAsync(List<string> images, string productId);

        Task UpdateProductAsync(ProductDTO product);

        Task DeleteImageAsync(string productId);
    }
}
