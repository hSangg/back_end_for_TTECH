using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.DTO;
using tech_project_back_end.Services.IService;
using Microsoft.Extensions.Logging;

namespace tech_project_back_end.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] CreateProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest("Product cannot be null.");
            }

            try
            {
                await _productService.AddProductAsync(productDTO);
                return CreatedAtAction(nameof(GetProductById), new { id = productDTO.ProductId }, productDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product.");
                return StatusCode(500, $"An error occurred while processing your request");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetFilteredProducts([FromBody] Filter filter)
        {
            try
            {
                var products = await _productService.GetFilteredProductsAsync(filter);
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering products.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return Ok("Product deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}/images")]
        public async Task<IActionResult> GetProductImages(string id)
        {
            try
            {
                var images = await _productService.GetProductImagesAsync(id);
                return Ok(images);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving images for product with ID: {Id}", id);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPost("{id}/images")]
        public async Task<IActionResult> AddImages(string id, List<IFormFile> formFiles)
        {
            if (formFiles == null || !formFiles.Any())
            {
                return BadRequest("No images provided.");
            }
             try
             {
                 await _productService.AddImagesAsync(formFiles, id);
                 return NoContent();
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, "Error occurred while adding images for product with ID: {Id}", id);
                 return StatusCode(500, "An error occurred while processing your request.");
             }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO product)
        {
            if (product == null)
            {
                return BadRequest("Product cannot be null.");
            }

            try
            {
                await _productService.UpdateProductAsync(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{productId}/images")]
        public async Task<IActionResult> DeleteImage(string productId)
        {
            try
            {
                await _productService.DeleteImageAsync(productId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting image for product ID: {ProductId}", productId);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
