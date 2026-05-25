using Microsoft.EntityFrameworkCore;
using WebApplication1.Repository.Interface;

namespace WebApplication1.Repository.Implementation
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;

        public CategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<List<Category>> GetAll()
        {
           var AllCategorys = await _appDbContext.Categories.ToListAsync();
            return AllCategorys;
        }
        public async Task<Category?> GetById(int id)
        {
            var OneCategory = await _appDbContext.Categories.FirstOrDefaultAsync(p => p.CategoryId == id);
            return OneCategory;
        }
        public async Task<Category> Create(Category category)
        {
            var CreateCategory = await _appDbContext.Categories.AddAsync(category);
            await _appDbContext.SaveChangesAsync();
            return CreateCategory.Entity;
        }
        public async Task<Category?> UpdateById(int ID, Category category)
        {
            var UpdateCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == ID);
            if (UpdateCategory == null) { return null; }
            UpdateCategory.CategoryName = category.CategoryName;
            UpdateCategory.CategoryDescription = category.CategoryDescription;
            await _appDbContext.SaveChangesAsync();
            return UpdateCategory;
        }
        public async Task<bool?> DeleteById(int id)
        {
            var DeleteCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (DeleteCategory == null) {return false; }
            _appDbContext.Remove(DeleteCategory);
            await _appDbContext.SaveChangesAsync();
            return true;
        }




    }
}
