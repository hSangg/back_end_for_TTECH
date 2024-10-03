using AutoMapper;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IMapper mapper)
        {
            _productRepository = productRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<TopSellerProductDTO>> GetTopSellerProducts(int count)
        {
            return await _productRepository.TopSeller(count);
        }

        public async Task AddProductAsync(ProductDTO productDTO)
        {
            var product = _mapper.Map<Product>(productDTO);
            await _productRepository.AddProductAsync(product);
        }

        public async Task<ProductDTO> GetProductByIdAsync(string id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }

        public async Task<FilteredProductResponse> GetFilteredProductsAsync(Filter filter)
        {
            return await _productRepository.GetFilteredProductsAsync(filter);
        }

        public async Task DeleteProductAsync(string productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<List<ImageDTO>> GetProductImagesAsync(string productId)
        {
            return await _productRepository.GetProductImagesAsync(productId);
        }

        public async Task AddImagesAsync(IFormFileCollection formFiles, string productId)
        {
            await _productRepository.AddImagesAsync(formFiles, productId);
        }

        public async Task UpdateProductAsync(ProductDTO productDTO)
        {
            await _productRepository.UpdateProductAsync(productDTO);
        }

        public async Task DeleteImageAsync(string productId, string fileName)
        {
            await _productRepository.DeleteImageAsync(productId, fileName);
        }
    }
}
