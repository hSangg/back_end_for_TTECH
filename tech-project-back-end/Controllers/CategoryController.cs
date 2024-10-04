using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.DTO;
using tech_project_back_end.Services;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving categories.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDTO categoryDto)
        {
            try
            {
                var result = await _categoryService.AddCategoryAsync(categoryDto);
                if (result)
                {
                    return Ok(categoryDto);
                }
                return BadRequest("Failed to add category.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while adding the category.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] string updatedCategoryName)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, updatedCategoryName);
                if (result)
                {
                    return Ok("Category updated successfully.");
                }
                return NotFound("Category not found.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while updating the category.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(string id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);
                if (category == null)
                {
                    return NotFound("Category not found.");
                }
                return Ok(category);
            }
            catch
            {
                return StatusCode(500, "An error occurred while retrieving the category.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (result)
                {
                    return Ok("Category deleted successfully.");
                }
                return NotFound("Category not found.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while deleting the category.");
            }
        }
    }
}
