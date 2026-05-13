using WebApplication1.DTOS;
using WebApplication1.Entities;
using WebApplication1.Models;
using WebApplication1.Repositories;
using WebApplication1.Repository.Interface;
using WebApplication1.Services.Interface;

namespace WebApplication1.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepoitory;

        public CategoryService(ICategoryRepository categoryRepoitory)
        {
            _categoryRepoitory = categoryRepoitory;
        }

        public async Task<List<CategoryModel>> GetAllAsync()
        {
            var GetAllCategorys = await _categoryRepoitory.GetAll();
            return GetAllCategorys.Select(x => new CategoryModel
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
            }).ToList();
        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {
            var GetByCanegoryId = await _categoryRepoitory.GetById(id);
            if (GetByCanegoryId == null) { return null; }
            return new CategoryModel
            {
                CategoryId = GetByCanegoryId.CategoryId,
                CategoryName = GetByCanegoryId.CategoryName,

            };

        }

        public async Task<CategoryModel> CreateAsync(CategoryDTO category)
        {
            var category1 = new Category
            {
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,

            };
            var categoryModel = await _categoryRepoitory.Create(category1);
            return new CategoryModel
            {
                CategoryId = categoryModel.CategoryId,
                CategoryName = categoryModel.CategoryName,

            };

        }

        public async  Task<CategoryDTO?> UpdateByIdAsync(int categoryId, Category category)
        {
            var Result = await _categoryRepoitory.UpdateById(categoryId , category);
            if (Result == null) { return null; }
            return new CategoryDTO
            {
                CategoryName = Result.CategoryName,
                CategoryDescription = Result.CategoryDescription,
            };
        }
        public async Task<bool?> DeleteByIdAsync(int id)
        {
            var Result = await _categoryRepoitory.DeleteById(id);
            return Result;
        }
    }
}
