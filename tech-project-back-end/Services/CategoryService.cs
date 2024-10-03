using AutoMapper;
using tech_project_back_end.DTO;
using tech_project_back_end.Models;
using tech_project_back_end.Repositories;
using tech_project_back_end.Repository.IRepository;

namespace tech_project_back_end.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<List<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<bool> AddCategoryAsync(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            await _categoryRepository.AddCategoryAsync(category);
            return true;
        }

        public async Task<bool> UpdateCategoryAsync(string id, string updatedCategoryName)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            category.CategoryName = updatedCategoryName;
            await _categoryRepository.UpdateCategoryAsync(category);
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(string id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            await _categoryRepository.DeleteCategoryAsync(id);
            return true;
        }
    }
}
