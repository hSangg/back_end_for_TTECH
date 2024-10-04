using tech_project_back_end.DTO;

namespace tech_project_back_end.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();

        Task<CategoryDTO> GetCategoryByIdAsync(string id);

        Task<bool> AddCategoryAsync(CategoryDTO categoryDto);

        Task<bool> UpdateCategoryAsync(string id, string updatedCategoryName);

        Task<bool> DeleteCategoryAsync(string id);
    }
}
