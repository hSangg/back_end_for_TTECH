using Microsoft.EntityFrameworkCore;
using tech_project_back_end.Data;
using tech_project_back_end.Models;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Category
                                 .Where(c => !c.IsDeleted)
                                 .ToListAsync();

        }
            public async Task<Category> GetCategoryByIdAsync(string id)
        {
            return await _context.Category.FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddCategoryAsync(Category category)
        {
            await _context.Category.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Category.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string id)
        {
            var category = await GetCategoryByIdAsync(id);
            if (category != null)
            {
                category.IsDeleted = true; 
                _context.Category.Update(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
