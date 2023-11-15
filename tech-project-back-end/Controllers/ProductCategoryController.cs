using Microsoft.AspNetCore.Http;
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
        public IActionResult AddNewProductCategory([FromBody] Product_Category pc)
        {
            _appDbContext.Product_Category.Add(pc);
            _appDbContext.SaveChanges();
            return Ok(pc);
        }

        [HttpPut("UpdateProductCategory")]
        public async Task<IActionResult> UpdateProductCategory([FromBody] Product_Category pc)
        {
            if (pc == null)
            {
                return BadRequest("Product_Category object is null");
            }

            try
            {
                var existingProductCategory = await _appDbContext.Product_Category
                    .FirstOrDefaultAsync(p => p.ProductId == pc.ProductId && p.CategoryId == pc.CategoryId);

                if (existingProductCategory == null)
                {
                    return NotFound("Product_Category not found");
                }

                existingProductCategory.CategoryId = pc.CategoryId;


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
            var result = _appDbContext.Product_Category.Where(pc => pc.ProductId == product_id);
            _appDbContext.Product_Category.RemoveRange(result);
            _appDbContext.SaveChanges();
            return Ok("Deleted product_category by product_id");
        }
    }
}
