using WebApplication1.DTOS;

using WebApplication1.Models;

namespace WebApplication1.Services.Interface
{
    public interface ICategoryService
    {
        public Task<List<CategoryModel>> GetAllAsync();
        public Task<CategoryModel?> GetByIdAsync(int id);
        public Task<CategoryModel> CreateAsync(CategoryDTO category);
        public Task<CategoryDTO?> UpdateByIdAsync(int categoryId, Category category);
        public Task<bool?> DeleteByIdAsync(int id);
    }
}
