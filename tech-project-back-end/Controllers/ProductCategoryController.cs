using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ProductCategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [HttpPost("AddNewProductCategory")]
        public IActionResult AddNewProductCategory(List<ProductCategory> product_Categories)
        {
            if (product_Categories == null) return BadRequest("Null");

            foreach (var pc in product_Categories)
            {
                _appDbContext.ProductCategory.Add(pc);
            }
            _appDbContext.SaveChanges();
            return Ok(product_Categories);
        }


        [HttpPut("UpdateProductCategory")]
        public async Task<IActionResult> UpdateProductCategory([FromBody] ProductCategory pc, string new_category_id)
        {
            if (pc == null)
            {
                return BadRequest("Product_Category object is null");
            }

            try
            {
                var existingProductCategory = await _appDbContext.ProductCategory
                    .FirstOrDefaultAsync(p => p.product_id == pc.product_id && p.category_id == pc.category_id);

                if (existingProductCategory == null)
                {
                    return NotFound("Product_Category not found");
                }

                _appDbContext.Remove(existingProductCategory);
                await _appDbContext.SaveChangesAsync();

                var newProductCategory = new ProductCategory
                {
                    product_id = pc.product_id,
                    category_id = new_category_id
                };

                _appDbContext.Add(newProductCategory);
                await _appDbContext.SaveChangesAsync();

                return Ok("Product_Category updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("RemoveProductCategory")]
        public IActionResult AddNewProductCategory(string product_id)
        {
            var result = _appDbContext.ProductCategory.Where(pc => pc.product_id == product_id);
            _appDbContext.ProductCategory.RemoveRange(result);
            _appDbContext.SaveChanges();
            return Ok("Deleted product_category by product_id");
        }
    }
}
