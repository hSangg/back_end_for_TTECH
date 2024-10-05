using tech_project_back_end.DTO;
using tech_project_back_end.Models;

namespace tech_project_back_end.Repository.IRepository
{
    public interface IProductRepository
    {
        Task<List<TopSellerProductDTO>> TopSeller(int count);

        Task AddProductAsync(Product product);

        Task<ProductDTO> GetProductByIdAsync(string id);

        Task<FilteredProductResponse> GetFilteredProductsAsync(Filter filter);

        Task DeleteProductAsync(string productId);

        Task<List<ImageDTO>> GetProductImagesAsync(string productId);

        Task AddImagesAsync(IFormFileCollection formFiles, string productId);

        Task UpdateProductAsync(ProductDTO product);

        Task DeleteImageAsync(string productId, string fileName);
    }
}
