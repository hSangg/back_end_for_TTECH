using AutoMapper;
using CloudinaryDotNet;
using tech_project_back_end.DTO;
using tech_project_back_end.Helpter;
using tech_project_back_end.Models;
using tech_project_back_end.Models.ViewModel;
using tech_project_back_end.Repository.IRepository;
using tech_project_back_end.Services.IService;

namespace tech_project_back_end.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger, IMapper mapper, Cloudinary cloudinary)
        {
            _productRepository = productRepository;
            _logger = logger;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<List<TopSellerProductDTO>> GetTopSellerProducts(int count)
        {
            return await _productRepository.TopSeller(count);
        }

        public async Task AddProductAsync(CreateProductDTO productDTO)
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

        public async Task<List<ProductBySearchQueryModel>> GetProductBySearchQueryAsync(string searchQuery)
        {
            string keyword = searchQuery.ToLower().Trim();

            var products = await _productRepository.GetProductBySearchQuery(keyword);

            return products;
        }

        public async Task DeleteProductAsync(string productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<List<ImageDTO>> GetProductImagesAsync(string productId)
        {
            var result = await _productRepository.GetProductImagesAsync(productId);
            var productImages = _mapper.Map<List<ImageDTO>>(result);
            return productImages;
        }

        public async Task AddImagesAsync(List<string> images, string productId)
        {
            foreach (var imageUrl in images)
            {
                var imageToCreate = new CreateImageDTO
                {
                    ProductId = productId,
                    ImageHref = imageUrl
                };

                var imageOfProduct = _mapper.Map<Image>(imageToCreate);
                await _productRepository.AddImageAsync(imageOfProduct);
            }
        }


        public async Task UpdateProductAsync(ProductDTO productDTO)
        {
            await _productRepository.UpdateProductAsync(productDTO);
        }

        public async Task DeleteImageAsync(string productId)
        {
            await _productRepository.DeleteImageAsync(productId);
        }
    }
}
