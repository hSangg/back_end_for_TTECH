using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public IActionResult AddProduct(Product product)
        {
            // Generate a unique ID for the product
            product.Id = Guid.NewGuid().ToString();

            _appDbContext.Products.Add(product);
            _appDbContext.SaveChanges();

            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _appDbContext.Products.ToListAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(); // Product with the given ID not found.
            }
            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductById(int id)
        {
            var product = await _appDbContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(); // Product with the given ID not found.
            }

            _appDbContext.Products.Remove(product);
            await _appDbContext.SaveChangesAsync();

            return Ok(); // Product deleted successfully.
        }
    }
}
