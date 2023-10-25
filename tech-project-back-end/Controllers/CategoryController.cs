using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var categoryList = _appDbContext.Category.ToListAsync();
            return Ok(categoryList);
        }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            category.category_id = Guid.NewGuid().ToString()[..15];
            _appDbContext.Add(category);
            _appDbContext.SaveChanges();

            return Ok(category);
        }

        [HttpGet("{id}")]
        public IActionResult GetCategory(string id) {
            var category = _appDbContext.Category.FirstOrDefault(c => c.category_id == id);
            if (category == null)
            {
                return NotFound("Category not found");
            } 

            return Ok(category);    
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(string id) {
            var category = _appDbContext.Category.FirstOrDefault(c => c.category_id == id);
            if (category == null) { return NotFound("Category not found"); }

            _appDbContext.Category.Remove(category);
            _appDbContext.SaveChanges();
            
            return Ok("Category deleted successfully");
        }

       


        
    }
}
