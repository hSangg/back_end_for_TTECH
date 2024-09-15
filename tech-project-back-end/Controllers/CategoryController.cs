using Microsoft.AspNetCore.Mvc;
using tech_project_back_end.Data;
using tech_project_back_end.Models;

namespace tech_project_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public CategoryController(AppDbContext appDbContext)
        {
            this._appDbContext = appDbContext;
        }

        [HttpGet]
        public IActionResult GetAllCategory()
        {
            var categoryList = _appDbContext.Category.ToList();
            return Ok(categoryList);
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            _appDbContext.Add(category);
            _appDbContext.SaveChanges();

            return Ok(category);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCategory(string id, string updatedCategoryName)
        {
            var exitingCategory = _appDbContext.Category.FirstOrDefault(c => c.category_id == id);
            if (exitingCategory == null) { return NotFound("Category not found"); }

            exitingCategory.category_name = updatedCategoryName;
            _appDbContext.SaveChanges();

            return Ok(exitingCategory);

        }

        [HttpGet("{id}")]
        public IActionResult GetCategory(string id)
        {
            var category = _appDbContext.Category.FirstOrDefault(c => c.category_id == id);
            if (category == null)
            {
                return NotFound("Category not found");
            }

            return Ok(category);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(string id)
        {
            var category = _appDbContext.Category.FirstOrDefault(c => c.category_id == id);
            if (category == null) { return NotFound("Category not found"); }

            _appDbContext.Category.Remove(category);
            _appDbContext.SaveChanges();

            return Ok("Category deleted successfully");
        }







    }
}
