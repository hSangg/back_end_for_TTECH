using Microsoft.AspNetCore.Http;
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
       { _appDbContext = appDbContext; }

        [HttpPost]
        public IActionResult AddCategory(Category category)
        {
            // Generate a unique ID for the product
            category.id = Guid.NewGuid().ToString();

            _appDbContext.Category.Add(category);
            _appDbContext.SaveChanges();

            return Ok(category);
        }


    }
}
